using AutoMapper;
using BookNow.Application.DTOs.TheatreDTOs;
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
    
    }
}
