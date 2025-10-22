using AutoMapper;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Show;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Show;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Errors.Model;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.TheatreOwner.Controllers
{
    [Area("TheatreOwner")]
    [Route("TheatreOwner/Show")]
    [Authorize(Roles = "TheatreOwner")]
    public class ShowController : Controller
    {
        private readonly IShowService _showService;
        private readonly IMapper _mapper;

        public ShowController(IShowService showService, IMapper mapper)
        {
            _showService = showService;
            _mapper = mapper;
        }


        [HttpGet("Shows")]
        public async Task<IActionResult> Shows([FromQuery] int screenId)
        {
           
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerId)) { return Forbid(); }

            try
            {
               
                var showListDto = await _showService.GetScreenShowsByOwnerAsync(screenId, ownerId);

                var viewModel = _mapper.Map<ScreenShowListVM>(showListDto);

                return View(viewModel);
            }
            catch (NotFoundException)
            {
                return NotFound("Screen not found or access denied.");
            }
        }
    }
}