using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models.Interfaces
{
    public interface IMovieRepository : IRepository<Movie>
    {

        IEnumerable<Movie> GetAllMoviesByProducer(string producerId);

        Movie GetMovieByProducer(int movieId, string producerId);
      
    }
}
