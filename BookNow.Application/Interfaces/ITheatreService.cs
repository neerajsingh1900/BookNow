using BookNow.Application.DTOs.CommonDTOs;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Application.DTOs.TheatreDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface ITheatreService
    {
        Task<TheatreDetailsDTO> CreateTheatreAsync(CreateTheatreDTO dto);

        // Fetches theatres owned by a specific user (TheatreOwner)
        Task<IEnumerable<TheatreDetailsDTO>> GetOwnerTheatresAsync(string ownerId);

        // Handles creating the screen and generating all associated seats
        Task<ScreenDetailsDTO> AddScreenToTheatreAsync(CreateScreenDTO dto);

        // Utility method to get data for dropdowns
        Task<IEnumerable<CityDTO>> GetCitiesByCountryAsync(string countryCode);

        Task<IEnumerable<CountryDTO>> GetCountriesAsync();
    }

    // You'll need a simple City DTO in a shared DTO folder (e.g., CommonDTOs)
    public class CityDTO
    {
        public int CityId { get; set; }
        public string Name { get; set; } = null!;
    }
}
