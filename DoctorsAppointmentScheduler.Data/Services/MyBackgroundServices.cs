using DoctorsAppointmentScheduler.Data.Core;
using DoctorsAppointmentScheduler.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static System.Formats.Asn1.AsnWriter;

namespace DoctorsAppointmentScheduler.Data.Services
{
    public class MyBackgroundServices: BackgroundService
    {
        private readonly ILogger<MyBackgroundServices> _logger;
        private readonly IServiceProvider _serviceProvider;
        public MyBackgroundServices(IServiceProvider serviceProvider, ILogger<MyBackgroundServices> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
       
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BackgroundJob");
            if (!stoppingToken.IsCancellationRequested)
            {
                TimeSpan delay = TimeSpan.Zero;
                //TimeSpan delay = DateTime.Today.AddDays(1) - DateTime.Now;
                await Task.Delay(delay, stoppingToken);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var scopedService = scope.ServiceProvider.GetRequiredService<IMedicalStaffService>();
                    await scopedService.UpdateExpiredAppointments();
                }
            }
        }

    }
}
