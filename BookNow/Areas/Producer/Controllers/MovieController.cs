using BookNow.Application.DTOs.MovieDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BookNow.Web.Areas.Producer.Controllers
{
    [Area("Producer")]
    [Authorize(Roles = "Producer")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFileStorageService _fileStorage;

        public MovieController(IMovieService movieService, UserManager<IdentityUser> userManager, IFileStorageService fileStorage)
        {
            _movieService = movieService;
            _userManager = userManager;
            _fileStorage = fileStorage;
        }

        private string? GetCurrentProducerId() => User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public IActionResult Index() => View();

        public async Task<IActionResult> Upsert(int? id)
        {
            var movieDto = new MovieCreateDTO();
            var producerId = GetCurrentProducerId();
            if (string.IsNullOrEmpty(producerId))
                return Unauthorized("User is not authenticated.");

            ViewData["MovieId"] = id ?? 0;

            if (id == null || id == 0)
                return View(movieDto);

            try
            {
                var movieReadDto = await _movieService.GetProducerMovieByIdAsync(id.Value, producerId);

                movieDto.Title = movieReadDto.Title!;
                movieDto.Genre = movieReadDto.Genre!;
                movieDto.Language = movieReadDto.Language!;
                movieDto.Duration = movieReadDto.Duration!;
                movieDto.ReleaseDate = movieReadDto.ReleaseDate!;
                movieDto.PosterUrl = movieReadDto.PosterUrl;

                return View(movieDto);
            }
            catch (ApplicationValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(movieDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred: " + ex.Message);
                return View(movieDto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(MovieCreateDTO movieDto, int? id)
        {
            var producerId = GetCurrentProducerId();
            if (string.IsNullOrEmpty(producerId))
                return Unauthorized("User is not authenticated.");

            ViewData["MovieId"] = id ?? 0;

            if (!ModelState.IsValid)
                return View(movieDto);

            // Poster file validation
            if (movieDto.PosterFile != null && movieDto.PosterFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(movieDto.PosterFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("PosterFile", "Allowed file types: jpg, jpeg, png, webp.");
                    return View(movieDto);
                }

                if (movieDto.PosterFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("PosterFile", "Max file size is 5 MB.");
                    return View(movieDto);
                }

                using var stream = movieDto.PosterFile.OpenReadStream();
                movieDto.PosterUrl = await _fileStorage.SaveFileAsync(stream, movieDto.PosterFile.FileName, "posters");
            }

            try
            {
                if (id == null || id == 0)
                {
                    await _movieService.CreateMovieAsync(movieDto, producerId);
                    TempData["success"] = "Movie created successfully!";
                }
                else
                {
                    var updateDto = new MovieUpdateDTO
                    {
                        Title = movieDto.Title,
                        Genre = movieDto.Genre,
                        Language = movieDto.Language,
                        Duration = movieDto.Duration,
                        ReleaseDate = movieDto.ReleaseDate,
                        PosterUrl = movieDto.PosterUrl
                    };

                    await _movieService.UpdateProducerMovieAsync(id.Value, updateDto, producerId);
                    TempData["success"] = "Movie updated successfully!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(movieDto);
            }
            catch (ApplicationValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(movieDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred: " + ex.Message);
                return View(movieDto);
            }
        }
    }
}
