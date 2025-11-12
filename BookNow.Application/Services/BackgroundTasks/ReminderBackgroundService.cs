using BookNow.Application.DTOs.EventDTOs;
using BookNow.Application.Interfaces;
using BookNow.Application.RepoInterfaces;
using BookNow.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ReminderBackgroundService : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<ShowReminderEventDTO> _events = new();
    private readonly ILogger<ReminderBackgroundService> _logger;

    public ReminderBackgroundService(IMessageBus messageBus, IServiceProvider serviceProvider, ILogger<ReminderBackgroundService> logger)
    {
        _messageBus = messageBus;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _messageBus.Subscribe<ShowReminderEventDTO>(ScheduleReminder);
        _messageBus.Subscribe<BookingConfirmedEventDTO>(SendConfirmationEmail);
    }
    public Task ScheduleReminder(ShowReminderEventDTO reminder)
    {
        _logger.LogInformation(
           "Scheduled reminder for BookingId {BookingId} at {TriggerAt}",
           reminder.BookingId,
           reminder.TriggerAtUtc);

        _events.Add(reminder);
        return Task.CompletedTask;
    }

    public async Task SendConfirmationEmail(BookingConfirmedEventDTO confirmation)
    {
        _logger.LogInformation("Sending confirmation email for BookingId {BookingId} to {Email}",
            confirmation.BookingId, confirmation.UserEmail);

       
        using var scope = _serviceProvider.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        string subject = $"Booking Confirmed: {confirmation.MovieTitle}";
        string body = $@"
            <h1>Your Booking is Confirmed!</h1>
            <p>Thank you for booking with us. Here are your details:</p>
            <ul>
                <li>Movie: <strong>{confirmation.MovieTitle}</strong></li>
                <li>Show Time: <strong>{confirmation.ShowTime:yyyy-MM-dd HH:mm tt}</strong></li>
                <li>Total Paid: <strong>{confirmation.CurrencySymbol}{confirmation.TotalAmount:N2}</strong></li>
            </ul>
            <p>Please find your e-ticket attached or available in your profile .</p>
        ";

        try
        {
            await emailService.SendEmailAsync(confirmation.UserEmail, subject, body);
            _logger.LogInformation("Confirmation email sent successfully for BookingId {BookingId}.", confirmation.BookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email for BookingId {BookingId}.", confirmation.BookingId);
        }
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reminder background service started.");
       
       

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var due = _events.Where(e => e.TriggerAtUtc <= now).ToList();
          //  _logger.LogDebug("Reminder service running. Queue count: {Count}", _events.Count);

            foreach (var reminder in due)
            {
                using var scope = _serviceProvider.CreateScope();
                var notifier = scope.ServiceProvider.GetRequiredService<IRealTimeNotifier>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

               
                var booking = await unitOfWork.Booking.GetAsync(
                    b => b.BookingId == reminder.BookingId,
                    includeProperties: "Show.Movie,BookingSeats.SeatInstance",
                    tracked: false);
               
                var user = await unitOfWork.ApplicationUser.GetAsync(
                            u => u.Id == reminder.UserId,
                               tracked: false);


                _logger.LogInformation(
        "Fetched user for reminder: User = {User}",
       user);

                if (booking != null && booking.BookingStatus == SD.BookingStatus_Confirmed)
                {
       _logger.LogInformation("Booking {BookingId}  confirmed  (status: {Status})",booking.BookingId,booking.BookingStatus);


                    // Email notification
                    await emailService.SendEmailAsync(
                        user?.Email!,
                        $"Reminder: {booking.Show.Movie.Title} starts in 10 minutes",
                        $"Your show {booking.Show.Movie.Title} starts at {booking.Show.StartTime:HH:mm}");
                }
                _logger.LogInformation("email send to {@UserIdking}", user?.Email);

                _events.Remove(reminder);
            }

            await Task.Delay(1000, stoppingToken);  
        }
    }
}
