using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.DataAccess.Repositories
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void LockUser(string userId)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.LockoutEnd = DateTime.UtcNow.AddYears(1);
            }
        }
    }
}
