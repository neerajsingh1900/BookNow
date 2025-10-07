using BookNow.DataAccess.Data;
using BookNow.DataAccess.Repositories;
using BookNow.Models;
using BookNow.Models.Interfaces;
using System;
using System.Threading.Tasks;

namespace BookNow.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        // Specific Repositories 
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IMovieRepository Movie { get; private set; }
        public IBookingRepository Booking { get; private set; }
        public ITheatreRepository Theatre { get; private set; }
        public IScreenRepository Screen => _screenRepository ??= new ScreenRepository(_db); 
        public ISeatRepository Seat => _seatRepository ??= new SeatRepository(_db);        


        public IPaymentTransactionRepository PaymentTransaction { get; private set; }

        // Generic Repositories 
        public IRepository<Country> Country { get; private set; }
        public IRepository<City> City { get; private set; }
      
        public IRepository<Show> Show { get; private set; }
        public IRepository<SeatInstance> SeatInstance { get; private set; }
        public IRepository<BookingSeat> BookingSeat { get; private set; }


     
        private IScreenRepository? _screenRepository;
        private ISeatRepository? _seatRepository;


        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            // 1. Initialize Specific Repositories
            ApplicationUser = new ApplicationUserRepository(_db);
            Movie = new MovieRepository(_db);
            Booking = new BookingRepository(_db);
            Theatre = new TheatreRepository(_db);
            PaymentTransaction = new PaymentTransactionRepository(_db);

            // 2. Initialize Generic Repositories
            Country = new Repository<Country>(_db);
            City = new Repository<City>(_db);
           
            Show = new Repository<Show>(_db);
            SeatInstance = new Repository<SeatInstance>(_db);
            BookingSeat = new Repository<BookingSeat>(_db);

        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}