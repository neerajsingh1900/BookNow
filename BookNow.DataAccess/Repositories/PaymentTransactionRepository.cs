using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Application.RepoInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.DataAccess.Repositories
{
    public class PaymentTransactionRepository : Repository<PaymentTransaction>, IPaymentTransactionRepository
    {
        private readonly ApplicationDbContext _db;

        public PaymentTransactionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public PaymentTransaction GetByBookingId(int bookingId)
        {
            return _db.PaymentTransactions.FirstOrDefault(p => p.BookingId == bookingId);
        }

        public PaymentTransaction GetByIdempotencyKey(string key)
        {
            return _db.PaymentTransactions.FirstOrDefault(p => p.IdempotencyKey == key);
        }

        public void UpdateTransactionStatus(int transactionId, string status, string? gatewayPaymentId)
        {
            var transactionFromDb = _db.PaymentTransactions.FirstOrDefault(p => p.PaymentTxnId == transactionId);
            if (transactionFromDb != null)
            {
                transactionFromDb.Status = status;
                transactionFromDb.GatewayPaymentId = gatewayPaymentId; 
                transactionFromDb.UpdatedAt = DateTime.Now;
                base.Update(transactionFromDb);
            }
        }
    }
}
