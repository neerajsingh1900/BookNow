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

       
        public int BookingId { get; set; }

      
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;

        
        public int SeatInstanceId { get; set; }

        
        [ForeignKey("SeatInstanceId")]
        public virtual SeatInstance SeatInstance { get; set; } = null!;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; } // decimal(10, 2)
    }
}
