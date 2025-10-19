using BookNow.Application.DTOs.ScreenDTOs;
using FluentValidation;

namespace BookNow.Application.Validation.ScreenValidations
{
    // Validator for the ScreenUpsertDTO, used by the application service layer
    public class ScreenUpsertValidator : AbstractValidator<ScreenUpsertDTO>
    {
        public ScreenUpsertValidator()
        {
            RuleFor(x => x.TheatreId)
                .NotEmpty().WithMessage("Theatre ID is required.");

            RuleFor(x => x.ScreenNumber)
                .NotEmpty().WithMessage("Screen identifier is required.")
                .MaximumLength(50).WithMessage("Screen identifier cannot exceed 50 characters.");

            RuleFor(x => x.NumberOfRows)
                .InclusiveBetween(1, 50).WithMessage("Number of rows must be between 1 and 50.");

            RuleFor(x => x.SeatsPerRow)
                .InclusiveBetween(1, 100).WithMessage("Seats per row must be between 1 and 100.");

            RuleFor(x => x.DefaultSeatPrice)
                .ExclusiveBetween(0.00m, 100000.00m).WithMessage("Default seat price must be greater than zero and less than 100,000.");
        }
    }
}
