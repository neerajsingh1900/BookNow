using AutoMapper;
using BookNow.Application.DTOs.ShowDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Application.Services;
using BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Show;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.TheatreOwner.Controllers.Api
{
    [Area("TheatreOwner")]
    [Route("TheatreOwner/api/show")]
    [Authorize(Roles = "TheatreOwner")]
    [ApiController]
    public class ShowApiController : ControllerBase
    {
        private readonly IShowService _showService;
        private readonly IMovieService _movieService;
        private readonly IMapper _mapper;

        public ShowApiController(IShowService showService, IMapper mapper, IMovieService movieService)
        {
            _showService = showService;
            _mapper = mapper;
            _movieService = movieService;
        }

        [HttpGet("GetShows")]
        [ServiceFilter(typeof(TheatreOwnershipFilter))]
        public async Task<IActionResult> GetShows([FromQuery] int screenId)
        {
            var shows = await _showService.GetShowsForScreenAsync(screenId);
            return Ok(new { data = shows });
        }

       
        [HttpGet("GetAllMovies")]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            return Ok(movies);
        }

   
    }
}
