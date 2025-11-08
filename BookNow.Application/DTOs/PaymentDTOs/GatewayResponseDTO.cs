using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.PaymentDTOs
{
    public class GatewayResponseDTO
    {
        public int BookingId { get; set; }
        public string Status { get; set; } = null!;
        public int CityId { get; set; }

        public string? Gateway { get; set; } 
        public string? GatewayOrderId { get; set; }
        public string? GatewayPaymentId { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? RawResponse { get; set; }

        public string IdempotencyKey { get; set; } = null!;
    }
}
