using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models
{
    public class SeatInstance
    {
        [Key]
        public int SeatInstanceId { get; set; } 

        
        public int ?ShowId { get; set; }

       
        [ForeignKey("ShowId")]
        public virtual Show Show { get; set; } = null!;

       
        public int SeatId { get; set; }

       
        [ForeignKey("SeatId")]
        public virtual Seat Seat { get; set; } = null!;

        public string State { get; set; } = "Available"; 

        public DateTime LastUpdated { get; set; } 

       
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

       
        public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    }
}
