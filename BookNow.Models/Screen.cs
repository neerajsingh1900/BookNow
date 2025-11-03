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
        public int ScreenId { get; set; } 
        public int TheatreId { get; set; }

        [ForeignKey("TheatreId")]
        public virtual Theatre Theatre { get; set; } = null!;

        public string ScreenNumber { get; set; } = null!;
        public int TotalSeats { get; set; } 

        [Column(TypeName = "decimal(10, 2)")]
        public decimal DefaultSeatPrice { get; set; } 
        public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
    }
}
