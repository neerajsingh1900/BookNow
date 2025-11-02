using BookNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.RepoInterfaces
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        void LockUser(string userId);
    }
}
