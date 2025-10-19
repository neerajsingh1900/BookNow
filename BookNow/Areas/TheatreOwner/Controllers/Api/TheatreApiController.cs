using AutoMapper;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
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
        public async Task<ActionResult<IEnumerable<TheatreListItemVM>>> GetOwnerTheatres()
        {
            var ownerId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
           
            var theatres = await _theatreService.GetOwnerTheatresAsync(ownerId);

            var vm = _mapper.Map<IEnumerable<TheatreListItemVM>>(theatres);
           
            return Ok(vm);
        }

        // POST: TheatreOwner/api/theatre
        [HttpPost]
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertTheatre([FromBody] TheatreUpsertDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
          
            try
            {
                TheatreDetailDTO theatreDto;
             
                if (dto.TheatreId.HasValue)
                {
                   
                    theatreDto = await _theatreService.UpdateTheatreAsync(dto.TheatreId.Value, dto, ownerId);
                
                }
                else
                { 
                    theatreDto = await _theatreService.AddTheatreAsync(ownerId, dto);
                    
                }
                var listItemVm = _mapper.Map<TheatreListItemVM>(theatreDto);
       
                if (dto.TheatreId.HasValue)
                    return Ok(listItemVm);     
                else
                    return CreatedAtAction(nameof(GetOwnerTheatres), new { id = theatreDto.TheatreId }, listItemVm);
            }
            catch (ApplicationValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid(); 
            }
            catch (Exception) 
            {
                return StatusCode(500, new { Message = "An unexpected error occurred while processing the theatre request." });
            }
        }
    }
}
