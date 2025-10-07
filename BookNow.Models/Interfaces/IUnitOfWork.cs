using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookNow.Models; // Ensure this is present if your models are here

namespace BookNow.Models.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IApplicationUserRepository ApplicationUser { get; }
        IMovieRepository Movie { get; }
        ITheatreRepository Theatre { get; }
        IBookingRepository Booking { get; }

        IRepository<Country> Country { get; }
        IRepository<City> City { get; }
      
        IRepository<Show> Show { get; }
        IRepository<SeatInstance> SeatInstance { get; }
        IRepository<BookingSeat> BookingSeat { get; }

        // FIX: Change to the specific repository interface for custom payment methods
        IPaymentTransactionRepository PaymentTransaction { get; }

        IScreenRepository Screen { get; } 
        ISeatRepository Seat { get; }    

        void Save();
        Task SaveAsync();
    }
}