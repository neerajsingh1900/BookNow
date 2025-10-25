using AutoMapper;
using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Application.Services;
using BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Screen;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Show;
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

        public ScreenController(IScreenService screenService, IMapper mapper, ITheatreService theatreService
            , IShowService showService)
        {
            _screenService = screenService;
            _mapper = mapper;
            _theatreService = theatreService;
            _showService = showService;
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


      
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Add(ScreenUpsertVM vm)
        {
            if (!ModelState.IsValid) return View("Upsert", vm);

            try
            {
                var dto = _mapper.Map<ScreenUpsertDTO>(vm);
                await _screenService.CreateScreenAsync(dto); 
                TempData["SuccessMessage"] = "Screen created successfully.";
                return RedirectToAction(nameof(Index), new { theatreId = vm.TheatreId });
            }
            catch (ApplicationValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
            }

            return View("Upsert", vm);
        }

       
     
        
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Update(int id, ScreenUpsertVM vm)
        {
            if (!ModelState.IsValid) return View("Upsert", vm);
            if (id != vm.ScreenId)
            {
                ModelState.AddModelError(string.Empty, "Screen ID mismatch.");
                return View("Upsert", vm);
            }

            try
            {
                var dto = _mapper.Map<ScreenUpsertDTO>(vm);
                await _screenService.UpdateScreenAsync(dto); 
                TempData["SuccessMessage"] = "Screen updated successfully.";
                return RedirectToAction(nameof(Index), new { theatreId = vm.TheatreId });
            }
            catch (ApplicationValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (NotFoundException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
            }

            return View("Upsert", vm);
        }
    }
}

