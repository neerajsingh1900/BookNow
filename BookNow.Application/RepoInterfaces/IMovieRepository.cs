using BookNow.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookNow.Application.RepoInterfaces

{

    public interface IMovieRepository : IRepository<Movie>
    {
        
        Task<IEnumerable<Movie>> GetAllMoviesByProducerAsync(string producerId);

        
        Task<IEnumerable<Movie>> GetCurrentlyShowingMoviesAsync(int cityId);

        Task<bool> ExistsByTitleAndDateAsync(string title, DateOnly releaseDate);
        Task<Movie?> GetMovieByProducerAsync(int movieId, string producerId);
    }
}