using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models
{
    public class BookingSeat
    {
        [Key]
        public int BookingSeatId { get; set; } // pk, increment

        // Foreign Key to Bookings (ref: > Bookings.BookingId)
        public int BookingId { get; set; }

        // Navigation property for Booking (Many-to-One)
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;

        // Foreign Key to SeatInstances (ref: > SeatInstances.SeatInstanceId)
        public int SeatInstanceId { get; set; }

        // Navigation property for SeatInstance (Many-to-One)
        [ForeignKey("SeatInstanceId")]
        public virtual SeatInstance SeatInstance { get; set; } = null!;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; } // decimal(10, 2)
    }
}
