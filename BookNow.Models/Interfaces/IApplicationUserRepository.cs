using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models.Interfaces
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        void LockUser(string userId);
    }
}
