using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Validation.BookingValidations
{
    public class GetSeatLayoutQueryValidator : AbstractValidator<int>
    {
        public GetSeatLayoutQueryValidator()
        {
            RuleFor(showId => showId)
                .GreaterThan(0).WithMessage("Invalid show identifier.");
        }
    }
}
