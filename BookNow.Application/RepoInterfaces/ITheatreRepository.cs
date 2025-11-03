using BookNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.RepoInterfaces
{
    public interface ITheatreRepository : IRepository<Theatre>
    {
        IEnumerable<Theatre> GetTheatresByOwner(string ownerId);
        Task<bool> IsNameConflictingAsync(string name, int? excludeTheatreId = null);
      
        Task<bool> IsEmailConflictingAsync(string email, int? theatreIdToExclude);
    }
}
