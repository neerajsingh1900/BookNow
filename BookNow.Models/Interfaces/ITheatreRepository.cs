using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models.Interfaces
{
    public interface ITheatreRepository : IRepository<Theatre>
    {
        IEnumerable<Theatre> GetTheatresByOwner(string ownerId, string? includeProperties = null);
        Task<bool> IsNameConflictingAsync(string name, int? excludeTheatreId = null);
      
        Task<bool> IsEmailConflictingAsync(string email, int? theatreIdToExclude);
    }
}
