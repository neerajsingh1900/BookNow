using BookNow.Application.Interfaces;
using BookNow.Application.Services;
using BookNow.DataAccess.Data;
using BookNow.DataAccess.Repositories;
using BookNow.DataAccess.UnitOfWork;
using BookNow.Models.Interfaces;
using BookNow.Utility;
using BookNow.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity with default IdentityUser (string key)
builder.Services.AddIdentity<IdentityUser,IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
}).AddDefaultTokenProviders()
.AddEntityFrameworkStores<ApplicationDbContext>();

//google stuff
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
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
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Register IUnitOfWork to its concrete implementation
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register the core business logic service for the Producer flow
builder.Services.AddScoped<IMovieService, MovieService>();

builder.Services.AddScoped<IFileStorageService, FileStorageService > ();

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
