using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.PaymentDTOs
{
    public class PaymentSummaryDTO
    {
        public int BookingId { get; set; }
        public string TicketNumber { get; set; } = null!;
        public string MovieTitle { get; set; } = null!;
        public string TheatreName { get; set; } = null!;
        public string ScreenName { get; set; } = null!;
        public DateTime ShowDateTime { get; set; }
        public List<string> SeatLabels { get; set; } = new List<string>();
        public decimal TotalAmount { get; set; }
        public string CurrencySymbol { get; set; } = null!;
        public string UserEmail { get; set; } = null!;

        public string? GatewayPaymentId { get; set; } 
        public string? Gateway { get; set; } 
        public string? CurrencyIsoCode { get; set; }

        public long HoldExpiryUnixTimeSeconds { get; set; }

        public long ServerUnixTimeSeconds { get; set; }
    }
}
