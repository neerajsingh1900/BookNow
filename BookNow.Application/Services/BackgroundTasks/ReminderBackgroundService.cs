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
    }
    public Task ScheduleReminder(ShowReminderEventDTO reminder)
    {
        _logger.LogInformation(
           "Scheduled reminder for BookingId {BookingId} at {TriggerAtUtc}",
           reminder.BookingId,
           reminder.TriggerAtUtc);

        _events.Add(reminder);
        return Task.CompletedTask;
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
                    _logger.LogInformation(
                           "Booking {BookingId}  confirmed  (status: {Status})",
                           booking.BookingId,
                           booking.BookingStatus);

                  
                    await notifier.NotifySeatUpdatesAsync(booking.Show.ShowId,
                        booking.BookingSeats.Select(bs => bs.SeatInstanceId).ToList(),
                        "Reminder");

                    // Email notification
                    await emailService.SendEmailAsync(
                        user?.Email,
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
