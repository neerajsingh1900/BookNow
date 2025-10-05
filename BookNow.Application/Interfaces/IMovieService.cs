using BookNow.Application.DTOs.MovieDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface IMovieService
    {
        // Producer specific CRUD methods:
        MovieReadDTO CreateMovie(MovieCreateDTO movieDto, string producerId);
        IEnumerable<MovieReadDTO> GetProducerMovies(string producerId);
        MovieReadDTO GetProducerMovieById(int movieId, string producerId);
        void UpdateProducerMovie(int movieId, MovieUpdateDTO movieDto, string producerId);
        void DeleteProducerMovie(int movieId, string producerId);

        IEnumerable<MovieReadDTO> GetAllMovies();


    }
}