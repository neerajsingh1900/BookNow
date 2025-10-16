//// BookNow.Application/Validation/TheatreUpsertDTOValidator.cs
//using BookNow.Application.DTOs.TheatreDTOs;
//using BookNow.Models.Interfaces; // Dependency on IUnitOfWork is acceptable here
//using FluentValidation;

//public class TheatreUpsertDTOValidator : AbstractValidator<TheatreUpsertDTO>
//{
//    private readonly IUnitOfWork _uow;

//    public TheatreUpsertDTOValidator(IUnitOfWork uow)
//    {
//        _uow = uow;

//        // 1. Semantic Check: City ID must exist in the database.
//        // This confirms the data integrity and security of the location field.
//        RuleFor(x => x.CityId)
//            .MustAsync(CityMustExist).WithMessage("The selected City is invalid or does not exist.");

//        // 2. Semantic Check: Theatre Name uniqueness (optional, depending on rule)
//        RuleFor(x => x.TheatreName)
//            .MustAsync(TheatreNameMustBeUnique).When(x => !x.TheatreId.HasValue).WithMessage("A theatre with this name already exists.");

//        // NOTE: Unique Email check can remain in the Service, but is cleaner here.
//        RuleFor(x => x.Email)
//                 .MustAsync((dto, email, token) => BeUniqueTheatreEmail(email, dto.TheatreId, token))
//                 .WithMessage("This email is already registered to another theatre.");
//    }

//    private async Task<bool> CityMustExist(int cityId, CancellationToken token)
//    {
//        // Requires a method on ICityRepository (or IUnitOfWork) to check for existence.
//        return await _uow.City.AnyAsync(c => c.CityId == cityId);
//    }

//    // Theatre Name unique check implementation
//    private async Task<bool> TheatreNameMustBeUnique(TheatreUpsertDTO dto, string name, CancellationToken token)
//    {
//        return !await _uow.Theatre.IsNameConflictingAsync(name, dto.TheatreId);
//    }

//    private async Task<bool> BeUniqueTheatreEmail(string email, int? theatreId, CancellationToken token)
//    {
//        // Similar repository method needed: IsEmailConflictingAsync
//        var existing = await _uow.Theatre.GetAsync(t => t.Email == email && t.TheatreId != theatreId);
//        return existing == null;
//    }
//}


using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Application.Interfaces;
using BookNow.Models.Interfaces;
using FluentValidation;

public class TheatreUpsertDTOValidator : AbstractValidator<TheatreUpsertDTO>
{
    private readonly IUnitOfWork _uow;

    public TheatreUpsertDTOValidator(IUnitOfWork uow)
    {
        _uow = uow;

        // 1. Required fields + length checks
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
}
