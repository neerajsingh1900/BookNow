//using BookNow.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;

//namespace BookNow.DataAccess.Data
//{
//    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole<int>, int>
//    {
//        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//            : base(options)
//        {
//        }

//        // OPTIONAL: You can add DbSets later when models are ready
//        // public DbSet<Movie> Movies { get; set; }
//        // public DbSet<Theatre> Theatres { get; set; }

//        protected override void OnModelCreating(ModelBuilder builder)
//        {
//            base.OnModelCreating(builder);

//            // GoogleId must be unique
//            builder.Entity<ApplicationUser>()
//                   .HasIndex(u => u.GoogleId)
//                   .IsUnique();
//        }
//    }
//}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookNow.Models;

namespace BookNow.DataAccess.Data
{
    // Default IdentityUser with string keys (same as fresh scaffold)
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public  DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
