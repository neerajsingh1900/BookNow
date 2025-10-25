using AutoMapper;
using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.DTOs.ScreenDTOs.BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Screen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Errors.Model;
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
        [ServiceFilter(typeof(TheatreOwnershipFilter))]
        public async Task<IActionResult> GetScreens(int theatreId)
        {
            var screens = await _screenService.GetScreensByTheatreIdAsync(theatreId);
            return Ok(screens);
        }
    }
}
