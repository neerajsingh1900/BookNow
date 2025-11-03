using BookNow.Application.DTOs.ShowDTOs;
using BookNow.Application.Interfaces;
using BookNow.Application.RepoInterfaces;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ShowCreationDTOValidator : AbstractValidator<ShowCreationDTO>
{
    private const int CLEANUP_BUFFER_MINUTES = 15;

    public ShowCreationDTOValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.StartTime)
            .Must(BeAtLeast30MinutesAhead)
            .WithMessage("Show start time must be at least 30 minutes in the future.");

        RuleFor(x => x)
            .MustAsync(async (dto, token) => await ScreenExistsAsync(dto.ScreenId, unitOfWork, token))
            .WithMessage(dto => $"Screen with ID {dto.ScreenId} not found.");

        RuleFor(x => x)
            .MustAsync(async (dto, token) => await MovieExistsAsync(dto.MovieId, unitOfWork, token))
            .WithMessage(dto => $"Movie with ID {dto.MovieId} not found.");

        RuleFor(x => x)
            .MustAsync(async (dto, token) =>
            {
                var movie = await unitOfWork.Movie.GetAsync(m => m.MovieId == dto.MovieId, tracked: false);
                return movie == null || dto.DurationMinutes >= (movie.Duration + CLEANUP_BUFFER_MINUTES);
            })
            .WithMessage("Total show duration cannot be less than movie run-time plus cleanup time.");

        RuleFor(x => x)
            .MustAsync(async (dto, token) => !await unitOfWork.Show.IsShowTimeConflictingAsync(dto.ScreenId, dto.StartTime, dto.StartTime.AddMinutes(dto.DurationMinutes)))
            .WithMessage("A show is already scheduled on this screen during the specified time.");
    }

    private bool BeAtLeast30MinutesAhead(DateTime startTime)
        => startTime >= DateTime.Now.AddMinutes(30);

    private async Task<bool> ScreenExistsAsync(int screenId, IUnitOfWork uow, CancellationToken token)
        => await uow.Screen.GetAsync(s => s.ScreenId == screenId, tracked: false) != null;

    private async Task<bool> MovieExistsAsync(int movieId, IUnitOfWork uow, CancellationToken token)
        => await uow.Movie.GetAsync(m => m.MovieId == movieId, tracked: false) != null;
}
