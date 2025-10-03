using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models
{
    public class Show
    {
        [Key]
        public int ShowId { get; set; } // pk, increment

        // Foreign Key to Screens (ref: > Screens.ScreenId)
        public int ScreenId { get; set; }

        // Navigation property for Screen (Many-to-One)
        [ForeignKey("ScreenId")]
        public virtual Screen Screen { get; set; } = null!;

        // Foreign Key to Movies (ref: > Movies.MovieId)
        public int MovieId { get; set; }

        // Navigation property for Movie (Many-to-One)
        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; } = null!;

        public DateTime StartTime { get; set; } // datetime (part of unique composite index)
        public DateTime EndTime { get; set; } // datetime

        // Navigation collections
        public virtual ICollection<SeatInstance> SeatInstances { get; set; } = new List<SeatInstance>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
