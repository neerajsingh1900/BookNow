using BookNow.Application.DTOs.CustomerDTOs.SearchDTOs;
using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Application.RepoInterfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.DataAccess.Repositories
{
    public class ShowRepository : Repository<Show>, IShowRepository
    {
        public ShowRepository(ApplicationDbContext db) : base(db) { }

        public async Task<bool> IsShowTimeConflictingAsync(int screenId, DateTime startTime, DateTime endTime, int? excludeShowId = null)
        {
           
            var query = dbSet.AsNoTracking()
                .Where(s => s.ScreenId == screenId)
                .Where(s =>
                    (startTime < s.EndTime && endTime > s.StartTime)
                );

            if (excludeShowId.HasValue)
            {
                query = query.Where(s => s.ShowId != excludeShowId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Show>> GetShowsByTheatreAsync(int theatreId, string? includeProperties = null)
        {
            var shows = dbSet
               .Include(s => s.Screen)
               .AsNoTracking()
               .Where(s => s.Screen.TheatreId == theatreId);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    shows = shows.Include(includeProp.Trim());
                }
            }

            return await shows.OrderBy(s => s.StartTime).ToListAsync();
        }


        public async Task<IEnumerable<Movie>> GetMoviesByCityAsync(int? cityId)
        {
            var now = DateTime.Now;

            var query = dbSet
                .AsNoTracking()
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                    .ThenInclude(sc => sc.Theatre)
                .Where(s => s.StartTime > now);

            if (cityId.HasValue)
                query = query.Where(s => s.Screen.Theatre.CityId == cityId.Value);

            var movies = await query
                .Select(s => s.Movie)
                .Distinct()     
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();

            return movies;
        }
        public async Task<IEnumerable<Show>> GetShowsForMovieAndCityAsync(int movieId, int cityId, DateOnly start, DateOnly end)
        {
            var startDate = start.ToDateTime(TimeOnly.MinValue);
            var endDate = end.ToDateTime(TimeOnly.MaxValue);


            return await
                dbSet.Include(s => s.Screen)
                .ThenInclude(scr => scr.Theatre)
                .Where(s => s.MovieId == movieId &&
                            s.Screen.Theatre.CityId == cityId &&
                            s.StartTime >= startDate &&
                            s.StartTime <= endDate)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<bool> AnyShowsForMovieAsync(int movieId)
        {
          
            return await dbSet
                .IgnoreQueryFilters() 
                .AnyAsync(s => s.MovieId == movieId);
        }

    }
}