using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.CustomerDTOs.BookingDTOs
{
    
    public class BookingHistoryDTO
    {
        public int BookingId { get; set; }

        
        public string MovieTitle { get; set; } = null!;
        public string MovieImageUrl { get; set; } = "/images/placeholder.jpg";
        public DateTime ShowDateTime { get; set; }
        public string TheatreName { get; set; } = null!;
        public string TheatreAddress { get; set; } = null!;
        public string ScreenName { get; set; } = null!;
        public string SeatLabels { get; set; } = null!; // e.g., "A5, A6"

        // Proof & Financials (Essential Details)
        public string TicketNumber { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string CurrencySymbol { get; set; } = "₹";
        public int TxnNo { get; set; }

        // Helper property for UI logic (Separation of Upcoming vs. Past)
        public bool IsUpcoming => ShowDateTime > DateTime.Now;

        public string City { get; set; } = null!; 
        public string Country { get; set; } = null!;
    }
}