using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Application.RepoInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.DataAccess.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext _db;

      
        public MovieRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
   public async Task<IEnumerable<Movie>> GetAllMoviesByProducerAsync(string producerId)
        {
             return await _db.Movies
                .Where(m => m.ProducerId == producerId)
                .AsNoTracking()
                .ToListAsync();
        }

      
        public async Task<IEnumerable<Movie>> GetCurrentlyShowingMoviesAsync(int cityId)
        {
             return await _db.Movies
                .Include(m => m.Shows)
                    .ThenInclude(s => s.Screen)
                        .ThenInclude(sc => sc.Theatre)
                .Where(m => m.Shows.Any(s =>
                    s.Screen.Theatre.CityId == cityId &&
                    s.StartTime > DateTime.Now))
                .AsNoTracking() 
                .Distinct()
                .ToListAsync();
        }
   public async Task<Movie?> GetMovieByProducerAsync(int movieId, string producerId)
        {
           
            return await _db.Movies
                .FirstOrDefaultAsync(m => m.MovieId == movieId && m.ProducerId == producerId);
        }

        public IEnumerable<Movie> GetAllMoviesByProducer(string producerId)
        {
            return _db.Movies.Where(m => m.ProducerId == producerId).ToList();
        }

        public IEnumerable<Movie> GetCurrentlyShowingMovies(int cityId)
        {
            return _db.Movies
                .Include(m => m.Shows)
                    .ThenInclude(s => s.Screen)
                        .ThenInclude(sc => sc.Theatre)
                .Where(m => m.Shows.Any(s =>
                    s.Screen.Theatre.CityId == cityId &&
                    s.StartTime > DateTime.Now))
                .Distinct()
                .ToList();
        }

        public async Task<bool> ExistsByTitleAndDateAsync(string title, DateOnly releaseDate)
        {
            return await _db.Movies
                .AsNoTracking()
                .AnyAsync(m => m.Title.ToLower() == title.ToLower() &&
                               m.ReleaseDate == releaseDate);
        }

    }
}