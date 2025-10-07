using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.ViewModels.Shared;
using BookNow.ViewModels.Theatre;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;


[Area("TheatreOwner")]
[Authorize(Roles = "TheatreOwner")]
public class TheatreController : Controller
{
    private readonly ITheatreService _theatreService;

    public TheatreController(ITheatreService theatreService)
    {
        _theatreService = theatreService;
    }

  
    private async Task<CreateTheatreDataViewModel> GetCountryDropdownData()
    {
        var countries = await _theatreService.GetCountriesAsync();
        return new CreateTheatreDataViewModel
        {
            Countries = countries.Select(c => new SelectListItem
            {
                
                Value = c.Code,
                Text = c.Name
            }).ToList()
        };
    }

    
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var viewModel = new CreateTheatreViewModel
        {
            DropdownData = await GetCountryDropdownData()
        };
        return View(viewModel);
    }

  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTheatreViewModel viewModel)
    {
        
        if (!ModelState.IsValid)
        {
           
            viewModel.DropdownData = await GetCountryDropdownData();
            return View(viewModel);
        }

  
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var dto = new CreateTheatreDTO
        {
            OwnerId = ownerId,
            TheatreName = viewModel.TheatreName,
            CityId = viewModel.CityId,
            Address = viewModel.Address,
            PhoneNumber = viewModel.PhoneNumber,
            Email = viewModel.Email
        };

        try
        {
            var createdTheatre = await _theatreService.CreateTheatreAsync(dto);

            TempData["SuccessMessage"] = $"Theatre '{createdTheatre.TheatreName}' created successfully and is Pending Admin Approval.";

            // Redirect to the dashboard or details page
            return RedirectToAction("Index");
        }
        catch (NotFoundException ex)
        {
            // CityId not found
            ModelState.AddModelError("", ex.Message);
        }
        catch (ValidationException ex)
        {
            // Duplicate Email, or other business validation failed
            ModelState.AddModelError("", ex.Message);
        }
        catch (Exception)
        {
            // Catch unexpected errors (database connection, etc.)
            ModelState.AddModelError("", "An unexpected error occurred during theatre creation. Please try again.");
        }

        // If an error occurred, reload dropdown data and return the view
        viewModel.DropdownData = await GetCountryDropdownData();
        return View(viewModel);
    }

    // GET: /TheatreOwner/Theatre/Index (Simple placeholder for the dashboard)
    public IActionResult Index()
    {
        return View();
    }

    // GET: /TheatreOwner/Theatre/Details/{id} (Placeholder for screen creation link)
    public IActionResult Details(int id)
    {
        return View();
    }
}