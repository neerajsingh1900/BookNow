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
        public int SeatInstanceId { get; set; } // pk, increment

        // Foreign Key to Shows (ref: > Shows.ShowId)
        public int ShowId { get; set; }

        // Navigation property for Show (Many-to-One)
        [ForeignKey("ShowId")]
        public virtual Show Show { get; set; } = null!;

        // Foreign Key to Seats (ref: > Seats.SeatId)
        public int SeatId { get; set; }

        // Navigation property for Seat (Many-to-One)
        [ForeignKey("SeatId")]
        public virtual Seat Seat { get; set; } = null!;

        public string State { get; set; } = "Available"; // varchar (e.g., Available, OnHold, Booked)

        public DateTime LastUpdated { get; set; } // datetime

        // rowversion in SQL Server maps to byte[] in C#
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        // Navigation collections
        public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    }
}
