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
        public int ShowId { get; set; } 

        
        public int ScreenId { get; set; }

       
        [ForeignKey("ScreenId")]
        public virtual Screen Screen { get; set; } = null!;

       
        public int MovieId { get; set; }

       
        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; } = null!;

        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; }

        public virtual ICollection<SeatInstance> SeatInstances { get; set; } = new List<SeatInstance>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
