using AutoMapper;
using BookNow.Application.Interfaces;
using BookNow.Application.Services;
using BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Screen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.TheatreOwner.Controllers
{
    [Area("TheatreOwner")]
    [Authorize(Roles = "TheatreOwner")]
    public class ScreenController : Controller
    {
        private readonly IScreenService _screenService;
        private readonly IMapper _mapper;
        private readonly ITheatreService _theatreService;

        public ScreenController(IScreenService screenService, IMapper mapper,ITheatreService theatreService)
        {
            _screenService = screenService;
            _mapper = mapper;
            _theatreService = theatreService;
        }

    
        [ServiceFilter(typeof(TheatreOwnershipFilter))]
        [Route("TheatreOwner/Screen/Index/{theatreId}")]
        public async Task<IActionResult> Index(int theatreId)
        {
            var ownerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var theatreDetail = await _theatreService.GetTheatreByIdAsync(theatreId, ownerId);

            ViewData["TheatreId"] = theatreId;
            ViewData["TheatreName"] = theatreDetail.TheatreName;

            return View();
        }

       
        [ServiceFilter(typeof(TheatreOwnershipFilter))]
        public async Task<IActionResult> Upsert(int theatreId, int? id)
        {
            ScreenUpsertVM viewModel;

            if (id.HasValue && id.Value > 0)
            {

                var screenDetailDto = await _screenService.GetScreenDetailsByIdAsync(id.Value);

                if (screenDetailDto == null || screenDetailDto.TheatreId != theatreId)
                {
                    return NotFound();
                }

                viewModel = _mapper.Map<ScreenUpsertVM>(screenDetailDto);
            }
            else
            {
                viewModel = new ScreenUpsertVM { TheatreId = theatreId };
            }

            return View(viewModel);
        }


    }
}
