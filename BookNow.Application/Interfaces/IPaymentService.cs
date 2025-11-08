using BookNow.Application.DTOs.PaymentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentSummaryDTO?> GetBookingSummaryAsync(int bookingId, int cityId);
        Task<string> ProcessGatewayResponseAsync(GatewayResponseDTO response);
        Task ReleaseSeatsAndLocksAsync(int bookingId);

    }
}
