using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models.Interfaces
{
    public interface IPaymentTransactionRepository : IRepository<PaymentTransaction>
    {
        PaymentTransaction GetByBookingId(int bookingId);
        PaymentTransaction GetByIdempotencyKey(string key);
        void UpdateTransactionStatus(int transactionId, string status, string? gatewayPaymentId);
    }
}
