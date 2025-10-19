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


        // POST: TheatreOwner/api/screen/add
        //[HttpPost("add")]
        //// Filter ensures vm.TheatreId belongs to the user (Clean Architecture Security)
        //[ServiceFilter(typeof(TheatreOwnershipFilter))]
        //public async Task<IActionResult> AddScreen([FromBody] ScreenUpsertVM vm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }


        //    var dto = _mapper.Map<ScreenUpsertDTO>(vm);

        //    try
        //    {
        //        var screenId = await _screenService.AddScreenAndSeatsAsync(dto.TheatreId, dto);
        //        return Ok(new { ScreenId = screenId, Message = "Screen and seats created successfully." });
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //    catch (ApplicationValidationException ex)
        //    {
        //        return NotFound(new { Message = ex.Message });
        //    }
        //}
   
    }
}
