using BookNow.Application.DTOs.MovieDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Models;
using BookNow.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;

       
        public MovieService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        private Movie MapToModel(MovieCreateDTO dto, string producerId)
        {
            return new Movie
            {
                Title = dto.Title,
                Genre = dto.Genre,
                Language = dto.Language,
                Duration = dto.Duration,
                ReleaseDate = dto.ReleaseDate.HasValue
                                ? DateOnly.FromDateTime(dto.ReleaseDate.Value)
                                : default,
                PosterUrl = dto.PosterUrl,
                ProducerId = producerId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }


        private MovieReadDTO MapToReadDTO(Movie model) => new MovieReadDTO
        {
            MovieId = model.MovieId,
            Title = model.Title ?? "Unknown",
            Genre = model.Genre ?? "Unknown",
            Language = model.Language ?? "Unknown",

            Duration = model.Duration,
            ReleaseDate = model.ReleaseDate.ToDateTime(TimeOnly.MinValue),

            PosterUrl = model.PosterUrl ?? string.Empty,

            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };

       

        public async Task<MovieReadDTO> CreateMovieAsync(MovieCreateDTO movieDto, string producerId)
        {
            if (movieDto == null)
                throw new ArgumentNullException(nameof(movieDto));
          
            if (string.IsNullOrEmpty(producerId))
                throw new UnauthorizedAccessException("ProducerId is required.");



            if (movieDto.ReleaseDate.HasValue)
            {
                DateOnly releaseDate = DateOnly.FromDateTime(movieDto.ReleaseDate.Value.Date);

                if (releaseDate < DateOnly.FromDateTime(DateTime.Today)) 
                {
                    throw new ValidationException("Movie release date cannot be in the past.");
                }

                bool duplicate = await _unitOfWork.Movie.ExistsByTitleAndDateAsync(
                    movieDto.Title!, 
                    releaseDate      
                );

                if (duplicate)
                {
                    throw new ValidationException($"A movie titled '{movieDto.Title}' already exists.");
                }
            }

            var movie = MapToModel(movieDto, producerId);

            await _unitOfWork.Movie.AddAsync(movie);
            await _unitOfWork.SaveAsync();

            return MapToReadDTO(movie);
        }

        public async Task<IEnumerable<MovieReadDTO>> GetAllMoviesAsync()
        {
            var movies = await _unitOfWork.Movie.GetAllAsync();
            return movies.Select(MapToReadDTO).ToList();
        }

        public async Task<IEnumerable<MovieReadDTO>> GetProducerMoviesAsync(string producerId)
        {
            if (string.IsNullOrEmpty(producerId))
                throw new UnauthorizedAccessException("ProducerId is required.");

            var movies = await _unitOfWork.Movie.GetAllMoviesByProducerAsync(producerId);
            return movies.Select(MapToReadDTO).ToList();
        }

        public async Task<MovieReadDTO> GetProducerMovieByIdAsync(int movieId, string producerId)
        {
           var movie = await _unitOfWork.Movie.GetMovieByProducerAsync(movieId, producerId);

            if (movie == null)
            {
                throw new ApplicationValidationException($"Movie with ID {movieId} not found or not owned by producer.");
            }

            return MapToReadDTO(movie);
        }

        public async Task UpdateProducerMovieAsync(int movieId, MovieUpdateDTO movieDto, string producerId)
        {
            if (movieDto == null)
                throw new ArgumentNullException(nameof(movieDto));
            if (string.IsNullOrEmpty(producerId))
                throw new UnauthorizedAccessException("ProducerId is required.");


            var movie = await _unitOfWork.Movie.GetMovieByProducerAsync(movieId, producerId);

            //FUTURE VALIDATION CHECK
          
            //if (movieDto.Duration.HasValue && movieDto.Duration.Value != movie.Duration)
            //{
                // Check 2: Query for any active or future shows linked to this movie.
                //bool hasActiveShows = await _unitOfWork.Show.AnyAsync(
                //    s => s.MovieId == movieId && s.StartTime >= DateTime.Now
                //);

                //if (hasActiveShows)
                //{
                    
                //    throw new ValidationException("Cannot change movie duration: active showtimes are currently scheduled.");
                //}
          //  }

            if (movie == null)
            {
                throw new ApplicationValidationException($"Movie with ID {movieId} not found or not owned by producer.");
            }

            if (!string.IsNullOrWhiteSpace(movieDto.Title))
                movie.Title = movieDto.Title;

            if (!string.IsNullOrWhiteSpace(movieDto.Genre))
                movie.Genre = movieDto.Genre;

            if (!string.IsNullOrWhiteSpace(movieDto.Language))
                movie.Language = movieDto.Language;

            if (movieDto.Duration.HasValue)
                movie.Duration = movieDto.Duration.Value;

            if (movieDto.ReleaseDate.HasValue)
            {
                var newDate = DateOnly.FromDateTime(movieDto.ReleaseDate.Value);
                if (newDate < DateOnly.FromDateTime(DateTime.Today))
                    throw new ValidationException("Release date cannot be in the past.");
                movie.ReleaseDate = newDate;
            }

            movie.PosterUrl = string.IsNullOrWhiteSpace(movieDto.PosterUrl) ? movie.PosterUrl : movieDto.PosterUrl;


            movie.UpdatedAt = DateTime.Now;

            _unitOfWork.Movie.Update(movie);
            await _unitOfWork.SaveAsync();

        }
       

        public async Task DeleteProducerMovieAsync(int movieId, string producerId)
        {
            if (string.IsNullOrEmpty(producerId))
                throw new UnauthorizedAccessException("ProducerId is required.");

            var movie = await _unitOfWork.Movie.GetMovieByProducerAsync(movieId, producerId);

            if (movie == null)
            {
                throw new ApplicationValidationException($"Movie with ID {movieId} not found or not owned by producer.");
            }

            _unitOfWork.Movie.Remove(movie);
            await _unitOfWork.SaveAsync();

        }
    }
}