using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookNow.Models;

namespace BookNow.DataAccess.Data
{
   
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public  DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theatre> Theatres { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<SeatInstance> SeatInstances { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            base.OnModelCreating(modelBuilder);

             modelBuilder.Entity<Show>()
                .HasOne(s => s.Screen)
                .WithMany(sc => sc.Shows)
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Show)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ShowId)
                .OnDelete(DeleteBehavior.Restrict);

           modelBuilder.Entity<SeatInstance>()
                .HasOne(si => si.Show)
                .WithMany(s => s.SeatInstances)
                .HasForeignKey(si => si.ShowId)
                .OnDelete(DeleteBehavior.Restrict);

            // Fix 4 (FINAL FIX): BookingSeat -> SeatInstance set to RESTRICT (Breaks the final cascade cycle involving transactional data)
            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.SeatInstance)
                .WithMany(si => si.BookingSeats)
                .HasForeignKey(bs => bs.SeatInstanceId)
                .OnDelete(DeleteBehavior.Restrict);


            // 2. Configure ApplicationUser (Custom Properties Index)
            // This ensures GoogleId is unique across existing users.
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.GoogleId)
                .IsUnique()
                .HasFilter("[GoogleId] IS NOT NULL");

            // 3. Configure Composite Unique Indexes

            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.ScreenId, s.SeatNumber })
                .IsUnique();

            modelBuilder.Entity<Show>()
                .HasIndex(s => new { s.ScreenId, s.StartTime })
                .IsUnique();

            // 4. Configure RowVersion (Timestamp) for Optimistic Concurrency
            modelBuilder.Entity<SeatInstance>()
                .Property(si => si.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Booking>()
                .Property(b => b.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<PaymentTransaction>()
                .Property(pt => pt.RowVersion)
                .IsRowVersion();

            // 5. Set Decimal Precision (for consistency)
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(10);
                property.SetScale(2);
            }
        }
    }
}
