using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models
{
    [Index(nameof(TicketNumber), IsUnique = true)] // Unique constraint
    [Index(nameof(IdempotencyKey), IsUnique = true)] // Unique constraint
    public class Booking
    {
        [Key]
        public int BookingId { get; set; } // pk, increment

        
        public string UserId { get; set; } = null!;

       
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

       
        public int ShowId { get; set; }

       
        [ForeignKey("ShowId")]
        public virtual Show Show { get; set; } = null!;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; } 

        public string BookingStatus { get; set; } = "Pending"; // varchar (e.g., Pending, Confirmed, Cancelled)

        public DateTime CreatedAt { get; set; } // datetime

      
        public string TicketNumber { get; set; } = null!; // varchar

        public string? TicketUrl { get; set; } // varchar
        public string? QRCodeUrl { get; set; } // varchar

       
        public string IdempotencyKey { get; set; } = null!; // varchar

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        // Navigation collections
        public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }
}
