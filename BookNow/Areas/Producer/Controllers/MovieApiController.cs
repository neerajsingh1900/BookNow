//using BookNow.Application.DTOs.MovieDTOs;
//using BookNow.Application.Exceptions;
//using BookNow.Application.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using System.Collections.Generic;


//[Area("Producer")]
//[Authorize(Roles = "Producer")]
//[ApiController]
//[Route("api/[area]/[controller]")]
//public class MovieApiController : ControllerBase
//{
//    private readonly IMovieService _movieService;
//    private readonly IFileStorageService _fileStorage;

//    public MovieApiController(IMovieService movieService, IFileStorageService fileStorage)
//    {
//        _movieService = movieService;
//        _fileStorage = fileStorage;
//    }

//    private string? GetCurrentProducerId()
//    {
//        return User?.FindFirstValue(ClaimTypes.NameIdentifier);
//    }


//    [HttpGet]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    public async Task<ActionResult<IEnumerable<MovieReadDTO>>> GetMovies()
//    {
//        var producerId = GetCurrentProducerId();
//        if (string.IsNullOrEmpty(producerId))
//            return Unauthorized("User is not authenticated.");

//        var movies = await _movieService.GetProducerMoviesAsync(producerId);
//        return Ok(movies);
//    }



//    [HttpDelete("{id:int}")]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> DeleteMovie(int id)
//    {
//        if (id <= 0)
//            return BadRequest("Invalid movie ID.");

//        var producerId = GetCurrentProducerId();
//        if (string.IsNullOrEmpty(producerId))
//            return Unauthorized("User is not authenticated.");

//        try
//        {

//            await _movieService.DeleteProducerMovieAsync(id, producerId);
//            return NoContent();
//        }
//        catch (NotFoundException ex)
//        {

//            return NotFound(ex.Message);
//        }
//        catch (Exception)
//        {

//            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred during movie deletion.");
//        }
//    }
//}

using BookNow.Application.DTOs.MovieDTOs;
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

    private string? GetCurrentProducerId() => User?.FindFirstValue(ClaimTypes.NameIdentifier);

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MovieReadDTO>>> GetMovies()
    {
        var producerId = GetCurrentProducerId();
        if (string.IsNullOrEmpty(producerId))
            return Unauthorized("User is not authenticated.");

        var movies = await _movieService.GetProducerMoviesAsync(producerId);
        return Ok(movies);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid movie ID.");

        var producerId = GetCurrentProducerId();
        if (string.IsNullOrEmpty(producerId))
            return Unauthorized("User is not authenticated.");

      
        await _movieService.DeleteProducerMovieAsync(id, producerId);
        return NoContent();
    }
}
