using BookNow.Application.DTOs.MovieDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Models;
using BookNow.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookNow.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MovieService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // --- Mapping Helpers (You would replace this with AutoMapper) ---
        private Movie MapToModel(MovieCreateDTO dto, string producerId) => new Movie
        {
            Title = dto.Title,
            Genre = dto.Genre,
            Language = dto.Language,
            Duration = dto.Duration,
            ReleaseDate = dto.ReleaseDate.HasValue ? DateOnly.FromDateTime(dto.ReleaseDate.Value) : default,
            PosterUrl = dto.PosterUrl,
            ProducerId = producerId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        private MovieReadDTO MapToReadDTO(Movie model) => new MovieReadDTO
        {
            MovieId = model.MovieId,
            Title = model.Title,
            Genre = model.Genre,
            Language = model.Language,
            Duration = model.Duration,
            ReleaseDate = model.ReleaseDate.ToDateTime(TimeOnly.MinValue),
            PosterUrl = model.PosterUrl,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
        // --------------------------------------------------------


        public MovieReadDTO CreateMovie(MovieCreateDTO movieDto, string producerId)
        {
            var movie = MapToModel(movieDto, producerId);

            _unitOfWork.Movie.Add(movie);
            _unitOfWork.Save();

            return MapToReadDTO(movie);
        }


        public IEnumerable<MovieReadDTO> GetAllMovies()
        {
            var movies = _unitOfWork.Movie.GetAll(); // fetch all movies
            return movies.Select(m => new MovieReadDTO
            {
                MovieId = m.MovieId,
                Title = m.Title,
                Genre = m.Genre,
                Language = m.Language,
                Duration = m.Duration,
                PosterUrl = m.PosterUrl,
                ReleaseDate = m.ReleaseDate.ToDateTime(TimeOnly.MinValue)
            });
           
        }

        public IEnumerable<MovieReadDTO> GetProducerMovies(string producerId)
        {
            var movies = _unitOfWork.Movie.GetAllMoviesByProducer(producerId);
            return movies.Select(MapToReadDTO).ToList();
        }

        public MovieReadDTO GetProducerMovieById(int movieId, string producerId)
        {
            // The repository enforces the crucial ownership check (MovieId + ProducerId)
            var movie = _unitOfWork.Movie.GetMovieByProducer(movieId, producerId);

            if (movie == null)
            {
                throw new NotFoundException($"Movie with ID {movieId} not found or not owned by producer.");
            }

            return MapToReadDTO(movie);
        }

        public void UpdateProducerMovie(int movieId, MovieUpdateDTO movieDto, string producerId)
        {
            // Retrieves the entity as TRACKED (ready for update)
            var movie = _unitOfWork.Movie.GetMovieByProducer(movieId, producerId);

            if (movie == null)
            {
                throw new NotFoundException($"Movie with ID {movieId} not found or not owned by producer.");
            }

            // Apply updates
            movie.Title = movieDto.Title ?? movie.Title;
            movie.Genre = movieDto.Genre ?? movie.Genre;
            movie.Language = movieDto.Language ?? movie.Language;
            if (movieDto.Duration.HasValue)
            {
                movie.Duration = movieDto.Duration.Value;
            }

            if (movieDto.ReleaseDate.HasValue)
            {
                movie.ReleaseDate = DateOnly.FromDateTime(movieDto.ReleaseDate.Value);
            }
            movie.PosterUrl = movieDto.PosterUrl ?? movie.PosterUrl;

            // Auditing and Change Tracking
            movie.UpdatedAt = DateTime.Now;

            // Since the entity is tracked, Save() commits the changes automatically.
            _unitOfWork.Save();
        }

        public void DeleteProducerMovie(int movieId, string producerId)
        {
            var movie = _unitOfWork.Movie.GetMovieByProducer(movieId, producerId);

            if (movie == null)
            {
                throw new NotFoundException($"Movie with ID {movieId} not found or not owned by producer.");
            }

            _unitOfWork.Movie.Remove(movie);
            _unitOfWork.Save();
        }
    }
}