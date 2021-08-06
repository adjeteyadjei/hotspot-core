using Humanizer;
using Hotvenues.Data;
using Hotvenues.Models;
using Hotvenues.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using hotvenues.Models;

namespace Hotvenues.Configurations
{
    public static class DataAccess
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("AppDbContext")));
            return services;
        }

        public static IServiceCollection InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IEnumService, EnumService>();
            services.AddScoped<ILiveStatusService, LiveStatusService>();
            services.AddScoped<ICommentStatusService, CommentStatusService>();
            services.AddScoped<ILikeStatusService, LikeStatusService>();
            services.AddScoped<IUpcomingEventService, UpcomingEventService>();
            return services;
        }

        public class DataMappingProfile : AutoMapper.Profile
        {
            public DataMappingProfile()
            {
                CreateMap<LiveStatus, LiveStatusDto>().ReverseMap();
                CreateMap<LikeStatus, LikeStatusDto>().ReverseMap();
                CreateMap<CommentStatus, CommentStatusDto>().ReverseMap();
                CreateMap<UpcomingEvent, UpcomingEventDto>().ReverseMap();
            }
        }
    }
}
