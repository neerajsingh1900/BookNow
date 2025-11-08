using BookNow.Application.DTOs.PaymentDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Validation.PaymentValidations
{
    public class GatewayResponseValidator : AbstractValidator<GatewayResponseDTO>
    {
        public GatewayResponseValidator()
        {
            RuleFor(x => x.BookingId).GreaterThan(0).WithMessage("Booking ID is required.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(BeValidStatus).WithMessage("Status must be 'success', 'failure', or 'timeout'.");
           
            RuleFor(x => x.IdempotencyKey)
                .NotEmpty().WithMessage("IdempotencyKey is required for safe payment processing.");

        }

        private bool BeValidStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return false;

            return status.Equals("success", StringComparison.OrdinalIgnoreCase) ||
                   status.Equals("failure", StringComparison.OrdinalIgnoreCase) ||
                   status.Equals("timeout", StringComparison.OrdinalIgnoreCase);
        }
    }
}
