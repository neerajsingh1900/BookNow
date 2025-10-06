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
    [Index(nameof(IdempotencyKey), IsUnique = true)] // Unique constraint
    public class PaymentTransaction
    {
        [Key]
        public int PaymentTxnId { get; set; } // pk, increment

        
        public int BookingId { get; set; }

      
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;

        public string Gateway { get; set; } = null!; // varchar
        public string GatewayOrderId { get; set; } = null!; // varchar
        public string? GatewayPaymentId { get; set; } // varchar

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; } // decimal(10, 2)

        public string Currency { get; set; } = null!; // varchar
        public string Status { get; set; } = "Pending"; // varchar

        public int AttemptNumber { get; set; } // int
        public DateTime CreatedAt { get; set; } // datetime
        public DateTime UpdatedAt { get; set; } // datetime

        // Store RawResponse as string/JSON
        [Column(TypeName = "nvarchar(MAX)")]
        public string? RawResponse { get; set; } // json

        
        public string IdempotencyKey { get; set; } = null!; // varchar

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}
