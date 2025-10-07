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
        void UpdateTheatreStatus(int theatreId, string status);
    }
}
