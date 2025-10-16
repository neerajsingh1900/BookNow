using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookNow.Models; 

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
      
        //IRepository<Show> Show { get; }
        //IRepository<SeatInstance> SeatInstance { get; }
        IRepository<BookingSeat> BookingSeat { get; }
        IScreenRepository Screen { get; } // New
        ISeatRepository Seat { get; } // New
        IShowRepository Show { get; } // New
        ISeatInstanceRepository SeatInstance { get; } // New

        IPaymentTransactionRepository PaymentTransaction { get; }

         

     
        Task SaveAsync();
    }
}