using AutoMapper;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Application.Interfaces;
using BookNow.Application.Mappings;
using BookNow.Application.Services;
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

// Add services to the container
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<BookNow.Application.Mappings.TheatreProfile>();
    cfg.AddProfile<BookNow.Web.Mappings.WebTheatreProfile>();

});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity with default IdentityUser (string key)
builder.Services.AddIdentity<IdentityUser,IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedAccount = true;
    

}).AddDefaultTokenProviders()
.AddEntityFrameworkStores<ApplicationDbContext>();

//google stuff
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        // FIX: Assert non-null using the Null-Forgiving Operator (!)
        // This tells the compiler: "I guarantee this configuration value exists."
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!; // Resolves CS8601 (Line 36)

        // Assuming this is line 37:
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!; // Resolves CS8601 (Line 37)
    });


// Configure cookie paths
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

// Add Razor Pages (for Identity UI)
builder.Services.AddRazorPages();


// Register IUnitOfWork to its concrete implementation
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register the core business logic service for the Producer flow
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ITheatreService, TheatreService>();


builder.Services.AddScoped<IFileStorageService, FileStorageService > ();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<TheatreOwnershipFilter>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<TheatreUpsertDTOValidator>();
var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();   

if (!app.Environment.IsDevelopment())
{
   
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
