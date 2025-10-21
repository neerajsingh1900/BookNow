using AutoMapper;
using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.DTOs.ScreenDTOs.BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Screen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.TheatreOwner.Controllers.Api
{
    [Area("TheatreOwner")]
    [Route("TheatreOwner/api/screen")]
    [Authorize(Roles = "TheatreOwner")]
    [ApiController]
    public class ScreenApiController : ControllerBase
    {
        private readonly IScreenService _screenService;
        private readonly IMapper _mapper;

        public ScreenApiController(IScreenService screenService, IMapper mapper)
        {
            _screenService = screenService;
            _mapper = mapper;
        }


        [HttpGet("list/{theatreId}")]
        public async Task<IActionResult> GetScreens(int theatreId)
        {
            var screens = await _screenService.GetScreensByTheatreIdAsync(theatreId);
            return Ok(screens);
        }


      //  POST: TheatreOwner/api/screen/add
       [HttpPost("add")]
       // Filter ensures vm.TheatreId belongs to the user (Clean Architecture Security)
       [ServiceFilter(typeof(TheatreOwnershipFilter))]
        public async Task<IActionResult> AddScreen([FromBody] ScreenUpsertVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ScreenUpsertDTO dto = _mapper.Map<ScreenUpsertDTO>(vm);

            try
            {
               var screenDetailDto = await _screenService.CreateScreenAsync(dto);
                return StatusCode(201, new
                {
                    ScreenId = screenDetailDto.ScreenId,
                    Message = "Screen and seats created successfully."
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (ApplicationValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Optional: log ex and return generic error for unexpected exceptions
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

    }
}
