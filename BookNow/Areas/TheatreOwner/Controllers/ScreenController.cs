using AutoMapper;
using BookNow.Application.Interfaces;
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

        public ScreenController(IScreenService screenService, IMapper mapper)
        {
            _screenService = screenService;
            _mapper = mapper;
        }

    
        [ServiceFilter(typeof(TheatreOwnershipFilter))]
        [Route("TheatreOwner/Screen/Index/{theatreId}")]
        public IActionResult Index(int theatreId)
        {
            ViewData["TheatreId"] = theatreId;
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
