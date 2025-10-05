using BookNow.Application.DTOs.MovieDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;


[Area("Producer")]
[Authorize(Roles = "Producer")]
[ApiController]
[Route("api/[area]/[controller]")] 
public class MovieApiController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IFileStorageService _fileStorage;

    public MovieApiController(IMovieService movieService, IFileStorageService fileStorage)
    {
       
        _movieService = movieService;
        _fileStorage = fileStorage;
    }

   
    private string GetCurrentProducerId()
    {
       
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

   
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<MovieReadDTO>> GetMovies()
    {
        var producerId = GetCurrentProducerId();
        var movies = _movieService.GetProducerMovies(producerId);
        return Ok(movies);
    }

   
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<MovieReadDTO> GetMovieById(int id)
    {
        var producerId = GetCurrentProducerId();
        try
        {
            var movie = _movieService.GetProducerMovieById(id, producerId);
            return Ok(movie);
        }
        catch (NotFoundException ex)
        {
            
            return NotFound(ex.Message);
        }
    }


   
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteMovie(int id)
    {
        var producerId = GetCurrentProducerId();
        try
        {
            _movieService.DeleteProducerMovie(id, producerId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}