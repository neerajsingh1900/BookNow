    using AutoMapper;
    using BookNow.Application.DTOs.CustomerDTOs.BookingDTOs;
    using BookNow.Application.DTOs.ScreenDTOs;
    using BookNow.Application.DTOs.TheatreDTOs;
    using BookNow.Application.Interfaces;
    using BookNow.Application.Mappings;
    using BookNow.Application.RepoInterfaces;
    using BookNow.Application.Services;
    using BookNow.Application.Services.Booking;
    using BookNow.Application.Validation.BookingValidations;
    using BookNow.Application.Validation.ScreenValidations;
    using BookNow.DataAccess.Data;
    using BookNow.DataAccess.Repositories;
    using BookNow.DataAccess.UnitOfWork;
    using BookNow.Web.Hubs;
    using BookNow.Utility;
    using BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters;
    using BookNow.Web.Infrastructure.Filters;
    using BookNow.Web.Middleware;
    using BookNow.Web.Services;
    using FluentValidation;
    using FluentValidation.AspNetCore;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using BookNow.Web.Customer.Infrastructure.Filters;
    using Serilog;
    using Serilog.Events;
    using BookNow.Application.Validation.PaymentValidations;
    using BookNow.Application.Services.BackgroundTasks;

var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug() 
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) 
        .CreateLogger();

    builder.Host.UseSerilog();
    builder.Host.UseSerilog();


    builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add<ValidationFilter>();
    });

    builder.Services.AddValidatorsFromAssemblyContaining<TheatreUpsertDTOValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<ShowCreationDTOValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateHoldCommandValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<GatewayResponseValidator>();
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
            googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;

       
            googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!; 
        });



    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = $"/Identity/Account/Login";
        options.LogoutPath = $"/Identity/Account/Logout";
        options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    });

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
        options.InstanceName = "BookNow_";
    });


     builder.Services.AddHttpClient<IExchangeRateService, RedisExchangeRateService>(client =>
      {
    var configuration = builder.Configuration;
    var baseUrl = configuration["FixerApi:BaseUrl"];
    if (string.IsNullOrEmpty(baseUrl))
    {
        throw new InvalidOperationException("Fixer API BaseUrl is missing in configuration. Check appsettings.json.");
    }

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});
    builder.Services.AddSingleton<IMessageBus, InMemoryMessageBus>();
builder.Services.AddHostedService<RateCacheWarmerJob>();
builder.Services.AddHostedService<ReminderBackgroundService>();
builder.Services.AddSignalR();
    builder.Services.AddRazorPages();


    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddSingleton<IRedisLockService, RedisLockService>();
builder.Services.AddScoped<IRealTimeNotifier, SignalRRealTimeNotifier>();

    builder.Services.AddScoped<IMovieService, MovieService>();
    builder.Services.AddScoped<ITheatreService, TheatreService>();
    builder.Services.AddScoped<IScreenService, ScreenService>();
    builder.Services.AddScoped<IShowService, ShowService>();
    builder.Services.AddScoped<IShowSearchService, ShowSearchService>();
    builder.Services.AddScoped<ISeatBookingService, SeatBookingService>();
    builder.Services.AddScoped<IPaymentService, PaymentService>();
    builder.Services.AddScoped<IBookingHistoryService, BookingHistoryService>();
    builder.Services.AddScoped<IProducerAnalyticsService, ProducerAnalyticsService>();



builder.Services.AddScoped<IFileStorageService, FileStorageService > ();
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    builder.Services.AddTransient<IEmailService, EmailSender>();
    builder.Services.AddScoped<TheatreOwnershipFilter>();
    builder.Services.AddScoped<BookingOwnershipFilter>();
    builder.Services.AddScoped<ILocationService, LocationService>();
    builder.Services.AddScoped<GetSeatLayoutQueryValidator>();
    builder.Services.AddScoped<IValidator<ScreenUpsertDTO>, ScreenUpsertValidator>();

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.MapHub<SeatMapHub>("/seatMapHub");
    app.UseRouting();
    app.UseMiddleware<LocationContextMiddleware>();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();

    app.MapControllerRoute(
        name: "default",
        pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

    app.Run();
