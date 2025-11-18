using BookNow.Application.DTOs.Analytics;
using BookNow.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookNow.Application.RepoInterfaces

{

    public interface IMovieRepository : IRepository<Movie>
    {
        
        Task<IEnumerable<Movie>> GetAllMoviesByProducerAsync(string producerId);

        Task<Movie?> GetMovieByProducerAsync(int movieId, string producerId);
        Task<IEnumerable<Movie>> GetCurrentlyShowingMoviesAsync(int cityId);

        Task<bool> ExistsByTitleAndDateAsync(string title, DateOnly releaseDate);
        Task<IEnumerable<RawRevenueAllMoviesDto>> GetProducerMoviesRevenueRawData(string producerUserId);

    }
}