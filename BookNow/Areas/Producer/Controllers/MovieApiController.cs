// BookNow.Web/Areas/Producer/Controllers/MovieController.cs

using BookNow.Application.DTOs.MovieDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;

// Defines the route and enforces the "Producer" role
[Area("Producer")]
[Authorize(Roles = "Producer")]
[ApiController]
[Route("api/[area]/[controller]")] // Route: api/Producer/Movie
public class MovieApiController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MovieApiController(IMovieService movieService)
    {
        // DI automatically provides the MovieService implementation
        _movieService = movieService;
    }

    // Helper to securely get the authenticated Producer's UserId
    private string GetCurrentProducerId()
    {
        // Retrieves the unique ID from the authenticated user's claims
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    // POST: api/Producer/Movie
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreateMovie([FromBody] MovieCreateDTO movieDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var producerId = GetCurrentProducerId();
        var createdMovie = _movieService.CreateMovie(movieDto, producerId);

        return CreatedAtAction(nameof(GetMovieById), new { id = createdMovie.MovieId }, createdMovie);
    }

    // GET: api/Producer/Movie
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<MovieReadDTO>> GetMovies()
    {
        var producerId = GetCurrentProducerId();
        var movies = _movieService.GetProducerMovies(producerId);
        return Ok(movies);
    }

    // GET: api/Producer/Movie/{id}
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
            // Handles both not found and unauthorized access (due to ownership check)
            return NotFound(ex.Message);
        }
    }

    // PUT: api/Producer/Movie/{id}
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateMovie(int id, [FromBody] MovieUpdateDTO movieDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var producerId = GetCurrentProducerId();
        try
        {
            _movieService.UpdateProducerMovie(id, movieDto, producerId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // DELETE: api/Producer/Movie/{id}
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