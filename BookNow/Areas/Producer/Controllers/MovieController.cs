using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookNow.Application.DTOs.MovieDTOs;
using BookNow.Application.Exceptions;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
namespace BookNow.Web.Areas.Producer.Controllers
{
    [Area("Producer")]
    [Authorize(Roles = "Producer")]
    public class MovieController : Controller 
    {
        private readonly IMovieService _movieService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFileStorageService _fileStorage;

        public MovieController(IMovieService movieService, UserManager<IdentityUser> userManager
            , IFileStorageService fileStorage)
        {
            _movieService = movieService;
            _userManager = userManager;
            _fileStorage = fileStorage;
        }

       
        private string GetCurrentProducerId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult Upsert(int? id)
        {
            MovieCreateDTO movieDto = new();
            string producerId = GetCurrentProducerId();

            if (id == null || id == 0)
            {
                
                ViewData["MovieId"] = 0;
                return View(movieDto);
            }
            else
            {
             
                try
                {
                    var movieReadDto = _movieService.GetProducerMovieById(id.Value, producerId);

                    
                    movieDto.Title = movieReadDto.Title;
                    movieDto.Genre = movieReadDto.Genre;
                    movieDto.Language = movieReadDto.Language;
                    movieDto.Duration = movieReadDto.Duration;
                    movieDto.ReleaseDate = movieReadDto.ReleaseDate;
                    movieDto.PosterUrl = movieReadDto.PosterUrl;

                    
                    ViewData["MovieId"] = id.Value;

                    return View(movieDto);
                }
                catch (NotFoundException)
                {
                    return NotFound();
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(MovieCreateDTO movieDto, int? id)
        {
            if (!ModelState.IsValid)
            {
                ViewData["MovieId"] = id;
                return View(movieDto);
            }

            string producerId = GetCurrentProducerId();

          
            if (movieDto.PosterFile != null && movieDto.PosterFile.Length > 0)
            {
               
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var fileExt = Path.GetExtension(movieDto.PosterFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExt))
                {
                    ModelState.AddModelError("PosterFile", "Allowed file types: jpg, jpeg, png, webp.");
                    ViewData["MovieId"] = id;
                    return View(movieDto);
                }

               
                if (movieDto.PosterFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("PosterFile", "Max file size is 5 MB.");
                    ViewData["MovieId"] = id;
                    return View(movieDto);
                }

               
                using var stream = movieDto.PosterFile.OpenReadStream();
                string savedUrl = await _fileStorage.SaveFileAsync(stream, movieDto.PosterFile.FileName, "posters");

            
                movieDto.PosterUrl = savedUrl;
            }

         
            if (id == null || id == 0)
            {

                // CREATE
                _movieService.CreateMovie(movieDto, producerId);
                TempData["success"] = "Movie created successfully!";
            }
            else
            {
                // UPDATE
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
    }
}
