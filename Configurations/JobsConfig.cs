using Hotvenues.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Hotvenues.Configurations
{
    public static class JobsConfig
    {
        public static IServiceCollection ConfigureBackgroundJobs(this IServiceCollection services)
        {
            //services.AddHostedService<MessagingJob>();
            return services;
        }
    }
}
