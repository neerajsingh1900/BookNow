using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookNow.Application.DTOs.MovieDTOs;
using BookNow.Application.Exceptions;
using System;
using Microsoft.AspNetCore.Identity;

namespace BookNow.Web.Areas.Producer.Controllers
{
    [Area("Producer")]
    [Authorize(Roles = "Producer")]
    public class MovieController : Controller // Inherits from Controller
    {
        private readonly IMovieService _movieService;
        private readonly UserManager<IdentityUser> _userManager;

        public MovieController(IMovieService movieService, UserManager<IdentityUser> userManager)
        {
            _movieService = movieService;
            _userManager = userManager;
        }

        // Helper to securely get the authenticated Producer's UserId
        private string GetCurrentProducerId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET /Producer/Movie/Index 
        public IActionResult Index()
        {
            return View();
        }

        // GET /Producer/Movie/Upsert?id={id} 
        public IActionResult Upsert(int? id)
        {
            MovieCreateDTO movieDto = new();
            string producerId = GetCurrentProducerId();

            if (id == null || id == 0)
            {
                // Create mode: Set MovieId to 0 for hidden input
                ViewData["MovieId"] = 0;
                return View(movieDto);
            }
            else
            {
                // Edit mode: fetch existing movie data
                try
                {
                    var movieReadDto = _movieService.GetProducerMovieById(id.Value, producerId);

                    // Map ReadDTO data to CreateDTO model for form display
                    movieDto.Title = movieReadDto.Title;
                    movieDto.Genre = movieReadDto.Genre;
                    movieDto.Language = movieReadDto.Language;
                    movieDto.Duration = movieReadDto.Duration;
                    movieDto.ReleaseDate = movieReadDto.ReleaseDate;
                    movieDto.PosterUrl = movieReadDto.PosterUrl;

                    // Pass the ID explicitly for the form Post action
                    ViewData["MovieId"] = id.Value;

                    return View(movieDto);
                }
                catch (NotFoundException)
                {
                    return NotFound();
                }
            }
        }

        // POST /Producer/Movie/Upsert (Handles the form submission and redirect)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(MovieCreateDTO movieDto, int? id)
        {
            // Server-side form validation
            if (ModelState.IsValid)
            {
                string producerId = GetCurrentProducerId();

                if (id == null || id == 0)
                {
                    // CREATE path
                    _movieService.CreateMovie(movieDto, producerId);
                    TempData["success"] = "Movie created successfully!";
                }
                else
                {
                    // UPDATE path
                    var updateDto = new MovieUpdateDTO
                    {
                        Title = movieDto.Title,
                        Genre = movieDto.Genre,
                        Language = movieDto.Language,
                        Duration = movieDto.Duration,
                        ReleaseDate = movieDto.ReleaseDate,
                        PosterUrl = movieDto.PosterUrl
                    };

                    try
                    {
                        _movieService.UpdateProducerMovie(id.Value, updateDto, producerId);
                        TempData["success"] = "Movie updated successfully!";
                    }
                    catch (NotFoundException)
                    {
                        return NotFound();
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, retain the MovieId for re-submission in Edit mode
            ViewData["MovieId"] = id;
            return View(movieDto);
        }
    }
}
