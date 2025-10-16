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
            // Service returns DTOs (Application Layer contract)
            var theatres = await _theatreService.GetOwnerTheatresAsync(ownerId);

            // Map DTOs to ViewModels (Web Layer projection)
            //  var vm = _mapper.Map<IEnumerable<TheatreListItemVM>>(theatres);
            var vm = _mapper.Map<IEnumerable<TheatreListItemVM>>(theatres);
            //var vm = theatres.Select(t => new TheatreListItemVM
            //{
            //    TheatreId = t.TheatreId,
            //    TheatreName = t.TheatreName,
            //    CityName = t.CityName,
            //    CountryName = t.CountryName,
            //    Status = t.Status,
            //    ScreenCount = t.ScreenCount
            //});
            return Ok(vm);
        }

        // POST: TheatreOwner/api/theatre
        [HttpPost]
  
        public async Task<IActionResult> UpsertTheatre([FromBody] TheatreUpsertVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Map ViewModel (Web Input) to DTO (Service Input)
                var dto = _mapper.Map<TheatreUpsertDTO>(vm);
            //var dto = new TheatreUpsertDTO
            //{
            //    TheatreId = vm.TheatreId,
            //    TheatreName = vm.TheatreName,
            //    CityId = vm.CityId,
            //    Address = vm.Address,
            //    PhoneNumber = vm.PhoneNumber,
            //    Email = vm.Email
            //};

            try
            {
                TheatreDetailDTO theatreDto;
               // var theatreDto = await _theatreService.AddTheatreAsync(ownerId, dto);
                if (vm.TheatreId.HasValue)
                {
                    // Update
                    theatreDto = await _theatreService.UpdateTheatreAsync(vm.TheatreId.Value, dto, ownerId);
                }
                else
                {
                    // Insert
                    theatreDto = await _theatreService.AddTheatreAsync(ownerId, dto);
                }
                var listItemVm = _mapper.Map<TheatreListItemVM>(theatreDto);
                //var listItemVm = new TheatreListItemVM
                //{
                //    TheatreId = theatreDto.TheatreId,
                //    TheatreName = theatreDto.TheatreName,
                //    CityName = theatreDto.CityName,
                //    CountryName = theatreDto.CountryName,
                //    Status = theatreDto.Status,
                //    ScreenCount = theatreDto.ScreenCount
                //};

                // RESTful response: 201 Created
                if (vm.TheatreId.HasValue)
                    return Ok(listItemVm);      // update
                else
                    return CreatedAtAction(nameof(GetOwnerTheatres), new { id = theatreDto.TheatreId }, listItemVm);
            }
            catch (ApplicationValidationException ex)
            {
                // Catch the aggregated semantic errors from the Application Layer
                // Returns HTTP 400 or 422 Unprocessable Entity (preferred for semantic errors)
                return BadRequest(new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                // Catch if the Service/Filter detected an authorization issue (e.g., user is not the owner of the ID they are trying to edit)
                return Forbid(); // HTTP 403
            }
            catch (Exception) // Catch all other unhandled runtime exceptions (DB errors, etc.)
            {
                // Never expose raw server errors. Log the exception details internally.
                return StatusCode(500, new { Message = "An unexpected error occurred while processing the theatre request." });
            }
        }
    }
}
