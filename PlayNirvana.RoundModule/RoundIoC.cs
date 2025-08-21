using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayNirvana.CommonModule.Interfaces;
using PlayNirvana.RoundModule.Application.BackgroundServices;
using PlayNirvana.RoundModule.Application.Repositories;
using PlayNirvana.RoundModule.Application.Services;
using PlayNirvana.RoundModule.External;
using PlayNirvana.RoundModule.Infrastructure.DataContext;
using PlayNirvana.RoundModule.Integrations;
using PlayNirvana.RoundModule.Presentation.RoundHub;

namespace PlayNirvana.RoundModule
{
    public static class RoundIoC
    {
        public static IServiceCollection RegisterRoundModule(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDbContext<RoundModuleDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PlayNirvanaConnectionString"));
            });

            services.AddScoped(typeof(IRoundModuleRepository<>), typeof(RoundModuleRepository<>));
            services.AddScoped<IRoundRepository, RoundRepository>();

            services.AddScoped<RoundService>();

            services.AddScoped<IRoundModuleExternal, RoundModuleExternal>();
            services.AddScoped<ITicketModuleIntegration, TicketModuleIntegration>();


            services.AddHostedService<RoundManagerService>();
            services.AddHostedService<RoundManagerService>();

            return services;
        }

        public static IEndpointRouteBuilder RegisterRoundApps(this IEndpointRouteBuilder host)
        {
            host.MapHub<RoundHub>("/roundHub");
            return host;
        }
    }
}
