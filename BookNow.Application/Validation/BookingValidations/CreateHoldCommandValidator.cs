using BookNow.Application.DTOs.CustomerDTOs.BookingDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Validation.BookingValidations
{
    public class CreateHoldCommandValidator : AbstractValidator<CreateHoldCommandDTO>
    {
        public CreateHoldCommandValidator()
        {
            RuleFor(x => x.ShowId)
                .GreaterThan(0).WithMessage("The show must be specified.");

            RuleFor(x => x.SeatInstanceIds)
                .NotEmpty().WithMessage("You must select at least one seat.")
                .Must(seats => seats.Count() <= 10).WithMessage("You can book a maximum of 10 seats per transaction.");

            RuleFor(x => x.SeatVersions.Count)
                .Equal(x => x.SeatInstanceIds.Count).WithMessage("Concurrency check data is incomplete. Please refresh and try again.");
        }
    }
}
