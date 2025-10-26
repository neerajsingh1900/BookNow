using BookNow.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookNow.Models.Interfaces
{
    public interface IShowRepository : IRepository<Show>
    {
        Task<bool> IsShowTimeConflictingAsync(int screenId, DateTime startTime, DateTime endTime, int? excludeShowId = null);
        Task<IEnumerable<Show>> GetShowsByTheatreAsync(int theatreId, string? includeProperties = null);

        Task<IEnumerable<Movie>> GetMoviesByCityAsync(int? cityId);
    }
}