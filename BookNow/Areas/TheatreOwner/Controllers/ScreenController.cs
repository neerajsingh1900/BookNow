using AutoMapper;
using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Application.Services;
using BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Screen;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Show;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SendGrid.Helpers.Errors.Model;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.TheatreOwner.Controllers
{
    [Area("TheatreOwner")]
    [Authorize(Roles = "TheatreOwner")]
    [ServiceFilter(typeof(TheatreOwnershipFilter))]
    public class ScreenController : Controller
    {
        private readonly IScreenService _screenService;
        private readonly IShowService _showService;
        private readonly IMapper _mapper;
        private readonly ITheatreService _theatreService;
        private readonly IValidator<ScreenUpsertDTO> _validator;
        public ScreenController(IScreenService screenService, IMapper mapper, ITheatreService theatreService
            , IShowService showService,
            IValidator<ScreenUpsertDTO> validator)
        {
            _screenService = screenService;
            _mapper = mapper;
            _theatreService = theatreService;
            _showService = showService;
            _validator = validator;
        }



        [Route("TheatreOwner/Screen/Index/{theatreId}")]
        public async Task<IActionResult> Index(int theatreId)
        {
            var ownerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var theatreDetail = await _theatreService.GetTheatreByIdAsync(theatreId, ownerId);

            ViewData["TheatreId"] = theatreId;
            ViewData["TheatreName"] = theatreDetail.TheatreName;

            return View();
        }



        public async Task<IActionResult> Upsert(int theatreId, int? id)
        {
            if (id is null or <= 0)
                return View(new ScreenUpsertVM { TheatreId = theatreId });

            var screen = await _screenService.GetScreenDetailsByIdAsync(id.Value);
            if (screen == null || screen.TheatreId != theatreId)
                return NotFound();

            return View(_mapper.Map<ScreenUpsertVM>(screen));
        }




        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Add(ScreenUpsertVM vm)
        {
            var dto = _mapper.Map<ScreenUpsertDTO>(vm);
            var result = await _validator.ValidateAsync(dto);

            if (!result.IsValid)
                result.Errors.ForEach(e => ModelState.AddModelError(e.PropertyName, e.ErrorMessage));

            if (!ModelState.IsValid) return View("Upsert", vm);

            await _screenService.CreateScreenAsync(dto);

            TempData["SuccessMessage"] = "Screen created successfully.";
            return RedirectToAction(nameof(Index), new { theatreId = vm.TheatreId });
        }

        
        


        [HttpPost]
        [ValidateAntiForgeryToken]
       
       public async Task<IActionResult> Update(int id, ScreenUpsertVM vm)
         {
            var dto = _mapper.Map<ScreenUpsertDTO>(vm);
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return View("Upsert", vm);
            }
            await _screenService.UpdateScreenAsync(dto);
            return RedirectToAction(nameof(Index), new { theatreId = vm.TheatreId });
}
    }
}

