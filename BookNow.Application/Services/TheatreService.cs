using BookNow.Application.DTOs.CommonDTOs;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Models;
using BookNow.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security;


namespace BookNow.Application.Services
{
    public class TheatreService : ITheatreService
    {
        private readonly IUnitOfWork _unitOfWork;
      
        public TheatreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TheatreDetailsDTO> CreateTheatreAsync(CreateTheatreDTO dto)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == dto.OwnerId);
            if (user == null || user.Role != "Owner")
            {
                throw new SecurityException("User does not have the required 'Owner' role to create a theatre.");
                // Note: Assuming you have a custom SecurityException or use a standard one like UnauthorizedAccessException
            }

            var existingTheatre = _unitOfWork.Theatre.Get(t => t.Email == dto.Email);
            if (existingTheatre != null)
            {
                throw new ValidationException($"A theatre with the email '{dto.Email}' already exists.");
            }

            var city = _unitOfWork.City.Get(c => c.CityId == dto.CityId);
            if (city == null)
            {
                throw new NotFoundException($"City with ID {dto.CityId} not found.");
            }
           
            
            // 2. Model Creation
            var theatre = new Theatre
            {
                OwnerId = dto.OwnerId,
                TheatreName = dto.TheatreName,
                CityId = dto.CityId,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Status = "Pending Approval" // Initial status
            };

            // 3. Persistence
            _unitOfWork.Theatre.Add(theatre);
            await _unitOfWork.SaveAsync();

            // 4. Return DTO
            return new TheatreDetailsDTO
            {
                TheatreId = theatre.TheatreId,
                TheatreName = theatre.TheatreName,
                CityName = city.Name,
                CountryName = city.Country?.Name ?? "N/A", // Requires 'Country' to be included in City repository's Get method
                Address = theatre.Address,
                Status = theatre.Status
            };
        }

        public async Task<ScreenDetailsDTO> AddScreenToTheatreAsync(CreateScreenDTO dto)
        {
            // 1. Validation (Theatre existence, ScreenNumber uniqueness)
            var theatre = _unitOfWork.Theatre.Get(t => t.TheatreId == dto.TheatreId);
            if (theatre == null)
            {
                throw new NotFoundException($"Theatre with ID {dto.TheatreId} not found.");
            }

            if (!_unitOfWork.Screen.IsScreenNumberUnique(dto.TheatreId, dto.ScreenNumber))
            {
                throw new ValidationException($"Screen number '{dto.ScreenNumber}' already exists in this theatre.");
            }
            if (dto.NumberOfRows <= 0 || dto.SeatsPerRow <= 0)
            {
                throw new ValidationException("Number of rows and seats per row must be positive.");
            }
            int totalSeats = dto.NumberOfRows * dto.SeatsPerRow;
            // 2. Create Screen Model
            var screen = new Screen
            {
                TheatreId = dto.TheatreId,
                ScreenNumber = dto.ScreenNumber,
                TotalSeats = totalSeats,
                DefaultSeatPrice = dto.DefaultSeatPrice
            };
            _unitOfWork.Screen.Add(screen);
            await _unitOfWork.SaveAsync(); // Save to get the generated ScreenId

            // 3. Generate and Persist Seats (The crucial part)
            var seats = GenerateSeatsForScreen(screen.ScreenId, dto.NumberOfRows, dto.SeatsPerRow);
            _unitOfWork.Seat.AddRange(seats);
            await _unitOfWork.SaveAsync(); // Save seats in bulk

            // 4. Return DTO
            return new ScreenDetailsDTO
            {
                ScreenId = screen.ScreenId,
                ScreenNumber = screen.ScreenNumber,
                TotalSeats = screen.TotalSeats,
                DefaultSeatPrice = screen.DefaultSeatPrice
            };
        }

        // Helper Method for generating seats (can be in a separate class, but keep it here for now)
        // Inside BookNow.Application/Services/TheatreService.cs

        private List<Seat> GenerateSeatsForScreen(int screenId, int numberOfRows, int seatsPerRow)
        {
            var seats = new List<Seat>();
            int seatIndex = 1;
            char startRowChar = 'A'; // Start row labeling from 'A'

            for (int row = 0; row < numberOfRows; row++)
            {
                // Calculate the current Row Label (A, B, C...)
                string rowLabel = ((char)(startRowChar + row)).ToString();

                for (int col = 1; col <= seatsPerRow; col++)
                {
                    seats.Add(new Seat
                    {
                        ScreenId = screenId,
                        SeatNumber = col.ToString(), // Column number is the Seat Number (1, 2, 3...)
                        RowLabel = rowLabel,         // Row label is the letter (A, B, C...)
                        SeatIndex = seatIndex        // Sequential index
                    });
                    seatIndex++;
                }
            }

            // Optional: Basic integrity check
            if (seatIndex - 1 != (numberOfRows * seatsPerRow))
            {
                throw new InvalidOperationException("Seat generation calculation error.");
            }

            return seats;
        }

        public async Task<IEnumerable<TheatreDetailsDTO>> GetOwnerTheatresAsync(string ownerId)
        {
            // Load Theatres, including City, Country, and Screens for the DTO
            var theatres = _unitOfWork.Theatre.GetTheatresByOwner(
                ownerId,
                includeProperties: "City.Country,Screens"
            );

            if (theatres == null)
            {
                return Enumerable.Empty<TheatreDetailsDTO>();
            }

            // Map Models to DTOs
            var theatreDTOs = theatres.Select(t => new TheatreDetailsDTO
            {
                TheatreId = t.TheatreId,
                TheatreName = t.TheatreName,
                Address = t.Address,
                Status = t.Status,

                CityName = t.City?.Name ?? "N/A",
                CountryName = t.City?.Country?.Name ?? "N/A",

                // Map list of screens
                Screens = t.Screens?.Select(s => new ScreenDetailsDTO
                {
                    ScreenId = s.ScreenId,
                    ScreenNumber = s.ScreenNumber,
                    TotalSeats = s.TotalSeats,
                    DefaultSeatPrice = s.DefaultSeatPrice
                }).ToList() ?? new List<ScreenDetailsDTO>()

            }).ToList();

            return await Task.FromResult(theatreDTOs);
        }

        public async Task<IEnumerable<CountryDTO>> GetCountriesAsync()
        {
            // 1. Fetch all countries (which is already limited to 5 by your DB data)
            var countries = _unitOfWork.Country.GetAll();

            // 2. Map Models to DTOs
            var countryDTOs = countries.Select(c => new CountryDTO
            {
                CountryId = c.CountryId,
                Name = c.Name,
                Code = c.Code
            }).ToList();

            return await Task.FromResult(countryDTOs);
        }

        // Implementation of GetOwnerTheatresAsync and GetCitiesByCountryAsync go here...
        // They will rely on ITheatreRepository.GetTheatresByOwner and IRepository<City>.GetAll, respectively.
        public async Task<IEnumerable<CityDTO>> GetCitiesByCountryAsync(string countryCode)
        {
            // 1. Fetch the Country entity by its Code to get its ID
            var country = _unitOfWork.Country.Get(c => c.Code.ToLower() == countryCode.ToLower());

            if (country == null)
            {
                return Enumerable.Empty<CityDTO>();
            }

            // 2. Fetch City entities filtered by the CountryId
            var cities = _unitOfWork.City.GetAll(c => c.CountryId == country.CountryId);

            // 3. Map Models to DTOs
            var cityDTOs = cities.Select(c => new CityDTO
            {
                CityId = c.CityId,
                Name = c.Name
            }).ToList();

            return await Task.FromResult(cityDTOs);
        }


    }


}