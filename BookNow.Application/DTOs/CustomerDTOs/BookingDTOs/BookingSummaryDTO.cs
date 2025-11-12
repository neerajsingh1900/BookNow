using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.CustomerDTOs.BookingDTOs
{
    public class BookingSummaryDTO
    {
        public int BookingId { get; set; }
        public string BookingStatus { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string TicketNumber { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string IdempotencyKey { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public int ShowId { get; set; }
        public string MovieTitle { get; set; } = null!;
        public DateTime ShowTime { get; set; }
        public List<int> SeatInstanceIds { get; set; } = new();
    }

}
