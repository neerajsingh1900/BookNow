using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.RepoInterfaces;
using FluentValidation;

namespace BookNow.Application.Validation.ScreenValidations
{
   
    public class ScreenUpsertValidator : AbstractValidator<ScreenUpsertDTO>
    {
        private readonly IUnitOfWork _uow;
        public ScreenUpsertValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TheatreId)
                .NotEmpty().WithMessage("Theatre ID is required.");

            RuleFor(x => x.ScreenNumber)
                .NotEmpty().WithMessage("Screen identifier is required.")
                .MaximumLength(50).WithMessage("Screen identifier cannot exceed 50 characters.");

            RuleFor(x => x.NumberOfRows)
                .InclusiveBetween(1, 50).WithMessage("Number of rows must be between 1 and 50.");

            RuleFor(x => x.SeatsPerRow)
                .InclusiveBetween(1, 100).WithMessage("Seats per row must be between 1 and 100.");
        
            
         RuleFor(x => x)
        .Must(dto => dto.NumberOfRows * dto.SeatsPerRow <= 5000) 
        .WithMessage("Total seats cannot exceed 5000 for a single screen.");

            RuleFor(x => x.ScreenNumber)
            .NotEmpty().WithMessage("Screen identifier is required.")
            .MaximumLength(50).WithMessage("Screen identifier cannot exceed 50 characters.")
            .Matches("^[a-zA-Z0-9].*[a-zA-Z0-9]$")
            .When(x => !string.IsNullOrWhiteSpace(x.ScreenNumber))
            .WithMessage("Screen identifier must start and end with an alphanumeric character and cannot be just punctuation.")
            .MustAsync(BeUniqueScreenNumber)
                .WithMessage(x => $"Screen number '{x.ScreenNumber}' already exists in this theatre.");

            RuleFor(x => x.DefaultSeatPrice)
                .ExclusiveBetween(0.01m, 99999.99m).WithMessage("Default seat price must be greater than zero and less than 100,000.");
     
          RuleFor(x => x.ScreenId)
      .MustAsync(NoRunningShows)
      .When(x => x.ScreenId.HasValue)
      .WithMessage("Cannot update screen while shows are scheduled.");
        }
        private async Task<bool> BeUniqueScreenNumber(ScreenUpsertDTO dto, string screenNumber, CancellationToken token)
        {
            return await _uow.Screen.IsScreenNumberUniqueAsync(dto.TheatreId, screenNumber, dto.ScreenId);
        }
        private async Task<bool> NoRunningShows(int? screenId,CancellationToken token)
        {
            if (!screenId.HasValue)
                return true;

            return !await _uow.Show.AnyAsync(s =>
                s.ScreenId == screenId.Value &&
                s.StartTime > DateTime.Now);
        }
    }
}
