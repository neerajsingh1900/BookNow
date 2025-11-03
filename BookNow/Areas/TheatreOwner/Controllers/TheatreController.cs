    using AutoMapper;
    using BookNow.Application.DTOs.TheatreDTOs;
    using BookNow.Application.Exceptions;
    using BookNow.Application.Interfaces;
    using BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters;
    using BookNow.Web.Areas.TheatreOwner.ViewModels.Theatre;
    using FluentValidation;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using FluentValidation.Results;

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
     
        
            public IActionResult Index()=> View();



        [ServiceFilter(typeof(TheatreOwnershipFilter))]
            public async Task<IActionResult> Upsert(int? id)
            {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var dto = id.HasValue
                ? _mapper.Map<TheatreUpsertDTO>(await _theatreService.GetTheatreByIdAsync(id.Value, ownerId))
                : new TheatreUpsertDTO();

            return View(dto);
        }


      
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(TheatreOwnershipFilter))]
        public async Task<IActionResult> Upsert(TheatreUpsertDTO dto)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await (dto.TheatreId.HasValue
                ? _theatreService.UpdateTheatreAsync(dto.TheatreId.Value, dto, ownerId)
                : _theatreService.AddTheatreAsync(ownerId, dto));

            return RedirectToAction(nameof(Index));
        }
    }
}
