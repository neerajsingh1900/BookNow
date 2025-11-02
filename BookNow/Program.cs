using AutoMapper;
using BookNow.Application.DTOs.CustomerDTOs.BookingDTOs;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Application.Interfaces;
using BookNow.Application.Mappings;
using BookNow.Application.Services;
using BookNow.Application.Services.Booking;
using BookNow.Application.Validation.BookingValidations;
using BookNow.Application.Validation.ScreenValidations;
using BookNow.DataAccess.Data;
using BookNow.DataAccess.Repositories;
using BookNow.DataAccess.UnitOfWork;
using BookNow.Models.Interfaces;
using BookNow.Utility;
using BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters;
using BookNow.Web.Middleware;
using BookNow.Web.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<BookNow.Application.Mappings.TheatreProfile>();
   cfg.AddProfile<BookNow.Web.Mappings.WebTheatreProfile>();
    cfg.AddProfile<BookNow.Application.Mappings.ScreenProfile>();
    cfg.AddProfile<BookNow.Application.Mappings.ShowProfile>();
    cfg.AddProfile<BookNow.Application.Mappings.ShowSearchProfile>();
    cfg.AddProfile<BookNow.Application.Mappings.LocationProfile>();
    cfg.AddProfile<BookNow.Application.Mappings.BookingProfile>();
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<IdentityUser,IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedAccount = true;
    
}).AddDefaultTokenProviders()
.AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!; // Resolves CS8601 (Line 36)

       
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!; // Resolves CS8601 (Line 37)
    });



builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});


builder.Services.AddRazorPages();


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ITheatreService, TheatreService>();
builder.Services.AddScoped<IScreenService, ScreenService>();
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IShowSearchService, ShowSearchService>();
builder.Services.AddScoped<ISeatBookingService, SeatBookingService>();

builder.Services.AddScoped<IFileStorageService, FileStorageService > ();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<TheatreOwnershipFilter>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<TheatreUpsertDTOValidator>();
builder.Services.AddScoped<ScreenUpsertValidator>();
builder.Services.AddScoped<CreateHoldCommandValidator>();
builder.Services.AddScoped<GetSeatLayoutQueryValidator>();
var app = builder.Build();


//app.UseMiddleware<ExceptionHandlingMiddleware>();

//if (!app.Environment.IsDevelopment())
//{

//    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseMiddleware<LocationContextMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
