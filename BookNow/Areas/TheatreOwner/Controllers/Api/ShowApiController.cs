using AutoMapper;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
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
        private readonly ITheatreService _theatreService;
        private readonly IMapper _mapper;

        public ShowApiController(ITheatreService theatreService, IMapper mapper)
        {
            _theatreService = theatreService;
            _mapper = mapper;
        }

        // POST: TheatreOwner/api/show/schedule
        [HttpPost("schedule")]
        // Filter ensures the owner has permission to schedule a show on the target screen
        [ServiceFilter(typeof(TheatreOwnershipFilter))]
        public async Task<IActionResult> ScheduleShow([FromBody] ShowCreationVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dto = _mapper.Map<ShowCreationDTO>(vm);

            try
            {
                // Core service logic handles time conflict check, show creation, and bulk seat inventory generation
                var show = await _theatreService.AddShowAsync(dto);
                return Ok(new { ShowId = show.ShowId, Message = "Show scheduled and inventory created successfully." });
            }
            catch (ValidationException ex)
            {
                // Catches conflicts like overlapping show times (robustness)
                return BadRequest(new { Message = ex.Message });
            }
            catch (ApplicationValidationException ex)
            {
                // Catches missing ScreenId or MovieId (data integrity)
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
