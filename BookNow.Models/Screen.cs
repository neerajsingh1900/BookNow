using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models
{
    public class Screen
    {
        [Key]
        public int ScreenId { get; set; } // pk, increment

        // Foreign Key to Theatres (ref: > Theatres.TheatreId)
        public int TheatreId { get; set; }

        // Navigation property for Theatre (Many-to-One)
        [ForeignKey("TheatreId")]
        public virtual Theatre Theatre { get; set; } = null!;

        public string ScreenNumber { get; set; } = null!; // varchar
        public int TotalSeats { get; set; } // int

        [Column(TypeName = "decimal(10, 2)")]
        public decimal DefaultSeatPrice { get; set; } // decimal(10, 2)

        // Navigation collections
        public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
    }
}
