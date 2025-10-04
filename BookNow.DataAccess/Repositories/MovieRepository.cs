using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void UpdateMovieStatus(int movieId, string newStatus)
        {
            var movieFromDb = _db.Movies.FirstOrDefault(m => m.MovieId == movieId);
            if (movieFromDb != null)
            {
                movieFromDb.UpdatedAt = DateTime.Now;
                base.Update(movieFromDb);
            }
        }
    }
}
