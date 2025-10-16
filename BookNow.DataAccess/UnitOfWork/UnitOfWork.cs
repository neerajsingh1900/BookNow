using BookNow.DataAccess.Data;
using BookNow.DataAccess.Repositories;
using BookNow.Models;
using BookNow.Models.Interfaces;
using System.Threading.Tasks;

namespace BookNow.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        private IApplicationUserRepository? _applicationUserRepository;
        private IMovieRepository? _movieRepository;
        private IBookingRepository? _bookingRepository;
        private ITheatreRepository? _theatreRepository;
        private IPaymentTransactionRepository? _paymentTransactionRepository;
        private IScreenRepository? _screenRepository;
        private ISeatRepository? _seatRepository;
        private IShowRepository? _showRepository;
        private ISeatInstanceRepository? _seatInstanceRepository;


        private IRepository<Country>? _countryRepository;
        private IRepository<City>? _cityRepository;
        //private IRepository<Show>? _showRepository;
        //private IRepository<SeatInstance>? _seatInstanceRepository;
        private IRepository<BookingSeat>? _bookingSeatRepository;

      
        public IApplicationUserRepository ApplicationUser => _applicationUserRepository ??= new ApplicationUserRepository(_db);
        public IMovieRepository Movie => _movieRepository ??= new MovieRepository(_db);
        public IBookingRepository Booking => _bookingRepository ??= new BookingRepository(_db);
        public ITheatreRepository Theatre => _theatreRepository ??= new TheatreRepository(_db);
        public IPaymentTransactionRepository PaymentTransaction => _paymentTransactionRepository ??= new PaymentTransactionRepository(_db);
        public IScreenRepository Screen => _screenRepository ??= new ScreenRepository(_db);
        public ISeatRepository Seat => _seatRepository ??= new SeatRepository(_db);
        public IShowRepository Show => _showRepository ??= new ShowRepository(_db);
        public ISeatInstanceRepository SeatInstance => _seatInstanceRepository ??= new SeatInstanceRepository(_db);

        public IRepository<Country> Country => _countryRepository ??= new Repository<Country>(_db);
        public IRepository<City> City => _cityRepository ??= new Repository<City>(_db);
        //public IRepository<Show> Show => _showRepository ??= new Repository<Show>(_db);
        //public IRepository<SeatInstance> SeatInstance => _seatInstanceRepository ??= new Repository<SeatInstance>(_db);
        public IRepository<BookingSeat> BookingSeat => _bookingSeatRepository ??= new Repository<BookingSeat>(_db);


        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
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