using System.Collections.Generic;
using System.Threading.Tasks;
using BookNow.Application.DTOs.MovieDTOs;
using System;

namespace BookNow.Application.Interfaces
{
    public interface IMovieService
    {
      

       
        Task<MovieReadDTO> CreateMovieAsync(MovieCreateDTO movieDto, string producerId);
        Task<IEnumerable<MovieReadDTO>> GetProducerMoviesAsync(string producerId);
        Task<MovieReadDTO> GetProducerMovieByIdAsync(int movieId, string producerId);
        Task UpdateProducerMovieAsync(int movieId, MovieUpdateDTO movieDto, string producerId);
        Task DeleteProducerMovieAsync(int movieId, string producerId);

        Task<IEnumerable<MovieReadDTO>> GetAllMoviesAsync();
    }
}