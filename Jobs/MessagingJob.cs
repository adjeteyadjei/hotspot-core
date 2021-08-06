using Hotvenues.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Hotvenues.Jobs
{
    public class MessagingJob : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public MessagingJob(IServiceScopeFactory serviceScopeFactory) => _serviceScopeFactory = serviceScopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var messageService = services.GetRequiredService<IMessageService>();
                    await messageService.ProcessOutwardMessages();
                    await Task.Delay(1000 * 60 * 1, stoppingToken); //Delay For 1 Minutes
                }
            }
        }
    }
}
