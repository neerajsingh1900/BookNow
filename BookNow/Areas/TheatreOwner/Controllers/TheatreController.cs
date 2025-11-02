using AutoMapper;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Theatre;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.TheatreOwner.Controllers
{
    [Area("TheatreOwner")]
    [Authorize(Roles = "TheatreOwner")]
    public class TheatreController : Controller
    {
        private readonly ITheatreService _theatreService;
        private readonly IMapper _mapper;
        public TheatreController(ITheatreService theatreService, IMapper mapper)
        {
            _theatreService = theatreService;

            _mapper = mapper;
        }
     
        
        public IActionResult Index()
        {
            return View();
        }

      
        [ServiceFilter(typeof(TheatreOwnershipFilter))]
        public async Task<IActionResult> Upsert(int? id)
        {
            TheatreUpsertDTO dto;
           
            if (id.HasValue)
            {
                var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var theatreDetail = await _theatreService.GetTheatreByIdAsync(id.Value, ownerId);

                
                dto = _mapper.Map<TheatreUpsertDTO>(theatreDetail);
            }
            else
            {
                dto = new TheatreUpsertDTO();
            }

            return View(dto);
        }   



      
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(TheatreOwnershipFilter))]
        public async Task<IActionResult> Upsert(TheatreUpsertDTO dto)
        {
            if (!ModelState.IsValid)
            {
               
                return View(dto);
            }

            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                TheatreDetailDTO theatreDetail;

                if (dto.TheatreId.HasValue)
                {
                    theatreDetail = await _theatreService.UpdateTheatreAsync(dto.TheatreId.Value, dto, ownerId);
                }
                else
                {
                    theatreDetail = await _theatreService.AddTheatreAsync(ownerId, dto);
                }

                
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while processing the theatre request.");
                return View(dto);
            }
        }



    }
}
