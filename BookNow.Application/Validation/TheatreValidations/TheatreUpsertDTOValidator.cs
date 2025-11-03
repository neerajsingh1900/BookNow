using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Application.Interfaces;
using BookNow.Application.RepoInterfaces;
using FluentValidation;

public class TheatreUpsertDTOValidator : AbstractValidator<TheatreUpsertDTO>
{
    private readonly IUnitOfWork _uow;

    public TheatreUpsertDTOValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(x => x.TheatreName)
            .NotEmpty().WithMessage("Theatre name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(250);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[0-9\s\-]{7,15}$").WithMessage("Invalid phone number.");

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("City is required.")
            .MustAsync(CityMustExist).WithMessage("Selected city does not exist.");

       
        RuleFor(x => x.TheatreName)
            .MustAsync(TheatreNameMustBeUnique)
            .When(x => !x.TheatreId.HasValue)
            .WithMessage("Theatre name already exists.");

        RuleFor(x => x.Email)
            .MustAsync(EmailMustBeUnique)
            .WithMessage("Email already registered.");

        RuleFor(x => x.TheatreId)
    .MustAsync(NoRunningShows)
    .When(x => x.TheatreId.HasValue) 
    .WithMessage("Cannot update theatre while shows are scheduled.");

    }

    private async Task<bool> CityMustExist(int cityId, CancellationToken token)
    {
        return await _uow.City.AnyAsync(c => c.CityId == cityId);
    }

    private async Task<bool> TheatreNameMustBeUnique(TheatreUpsertDTO dto, string name, CancellationToken token)
    {
        return !await _uow.Theatre.IsNameConflictingAsync(name, dto.TheatreId);
    }

    private async Task<bool> EmailMustBeUnique(TheatreUpsertDTO dto, string email, CancellationToken token)
    {
        var existing = await _uow.Theatre.GetAsync(t => t.Email == email && t.TheatreId != dto.TheatreId);
        return existing == null;
    }

    private async Task<bool> NoRunningShows(int? theatreId, CancellationToken token)
    {
        if (!theatreId.HasValue)
            return true;
      
        return !await _uow.Show.AnyAsync(s =>
            s.Screen.TheatreId == theatreId.Value &&
            s.StartTime > DateTime.Now);
    }


}
