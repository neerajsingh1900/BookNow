using AutoMapper;
using BookNow.Application.DTOs.ShowDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Application.Services;
using BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Show;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Errors.Model;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.TheatreOwner.Controllers
{

    [Area("TheatreOwner")]
    [Authorize(Roles = "TheatreOwner")]
    [ServiceFilter(typeof(TheatreOwnershipFilter))]
    public class ShowController : Controller
    {
        private readonly IShowService _showService;
        private readonly IMapper _mapper;

        public ShowController(IShowService showService, IMapper mapper)
        {
            _showService = showService;
            _mapper = mapper;
            
        }

      

        public async Task<IActionResult> Index([FromQuery] int screenId)
        {
            var screenMetadata = await _showService.GetScreenMetadataAsync(screenId);
            return View(screenMetadata);
        }

      
        public IActionResult Upsert(int screenId)
        {
            var dto = new ShowCreationDTO
            {
                ScreenId = screenId
            };
            return View(dto);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ShowCreationDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _showService.AddShowAsync(dto);
              
               TempData["SuccessMessage"] = "Show scheduled successfully! Seat inventory created.";
               
                return RedirectToAction("Index", new { screenId = dto.ScreenId });
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
            catch (ApplicationValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error: " + ex.Message);
                return View(dto);
            }
        }



    }
}