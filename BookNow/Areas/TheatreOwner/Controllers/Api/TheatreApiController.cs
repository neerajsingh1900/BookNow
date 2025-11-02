using AutoMapper;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters;
using BookNow.Web.Areas.TheatreOwner.ViewModels.Theatre;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.TheatreOwner.Controllers.Api
{
    [Area("TheatreOwner")]
    [Route("TheatreOwner/api/theatre")]
    [Authorize(Roles = "TheatreOwner")]
    [ApiController]
    public class TheatreApiController : ControllerBase
    {
        private readonly ITheatreService _theatreService;
        private readonly IMapper _mapper;

        public TheatreApiController(ITheatreService theatreService, IMapper mapper)
        {
            _theatreService = theatreService;
            _mapper = mapper;
        }

        // GET: TheatreOwner/api/theatre
        [HttpGet]
        [ServiceFilter(typeof(TheatreOwnershipFilter))]
        public async Task<ActionResult<IEnumerable<TheatreListItemVM>>> GetOwnerTheatres()
        {
            var ownerId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
           
            var theatres = await _theatreService.GetOwnerTheatresAsync(ownerId);

            var vm = _mapper.Map<IEnumerable<TheatreListItemVM>>(theatres);
           
            return Ok(vm);
        }
    }
}
