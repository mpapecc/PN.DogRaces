using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlayNirvana.CommonModule.DataContext;
using PlayNirvana.CommonModule.Interfaces;
using PlayNirvana.RoundModule.Application;
using PlayNirvana.RoundModule.Application.BackgroundServices;
using PlayNirvana.RoundModule.Application.Repositories;
using PlayNirvana.RoundModule.Application.Services;
using PlayNirvana.RoundModule.Common.Options;
using PlayNirvana.RoundModule.External;
using PlayNirvana.RoundModule.Infrastructure.DataContext;
using PlayNirvana.RoundModule.Integrations;
using PlayNirvana.RoundModule.Presentation.RoundHub;

namespace PlayNirvana.RoundModule
{
    public static class RoundModuleIoC
    {
        public static IServiceCollection RegisterRoundModule(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.RegisterInfrastructure(configuration)
                    .RegisterServices()
                    .RegisterBackgroundServices()
                    .RegisterOptions();

            return services;
        }

        private static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDbContext<RoundModuleDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PlayNirvanaConnectionString"), o =>
                {
                    o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, DbSchema.Rounds);
                });
            });

            services.AddScoped(typeof(IRoundModuleRepository<>), typeof(RoundModuleRepository<>));
            services.AddScoped<IRoundRepository, RoundRepository>();
            services.AddScoped<ITicketModuleIntegration, TicketModuleIntegration>();

            return services;
        }

        private static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<ActiveRoundCache>();
            services.AddSingleton<ScopeRunner>();

            services.AddScoped<RoundService>();
            services.AddScoped<RoundsGeneratorService>();
            services.AddScoped<RoundOutcomeService>();
            services.AddScoped<IRoundModuleExternal, RoundModuleExternal>();

            return services;
        }

        private static IServiceCollection RegisterBackgroundServices(this IServiceCollection services)
        {
            services.Configure<HostOptions>(hostOptions =>
            {
                hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            });

            services.AddHostedService<RoundsGenerator>();
            services.AddHostedService<ActiveRoundCacheInitializer>();
            services.AddHostedService<RoundManager>();

            return services;
        }

        private static IServiceCollection RegisterOptions(this IServiceCollection services)
        {
            services.AddOptions<RoundOptions>()
                    .BindConfiguration(nameof(RoundOptions))
                    .Validate(roundOptions =>
                    {
                        bool IsRoundDurationGreaterThenSegmentsDuration()
                        {
                            var requiredDuration = roundOptions.RaceDurationInSeconds + roundOptions.RoundLockBeforeRaceStart + roundOptions.MinimumRoundDurationBeforeLockInSeconds;
                            return roundOptions.RoundDurationInSeconds > requiredDuration;
                        }

                        return IsRoundDurationGreaterThenSegmentsDuration();

                    }, "Total round segments duration is greater then round duration or Duration. Change values in appsettings.json file")
                    .Validate(roundOptions =>
                    {
                        bool IsRoundDurationValid()
                        {
                            return 3600 % roundOptions.RoundDurationInSeconds == 0;
                        }

                        return IsRoundDurationValid();
                    }, "Round duration is not valid, should be set to satisfy 3600 % x == 0. Change values in appsettings.json file")
                    .ValidateOnStart();

            return services;
        }

        public static IEndpointRouteBuilder RegisterRoundApps(this IEndpointRouteBuilder host)
        {
            host.MapHub<RoundHub>("/roundHub");
            return host;
        }
    }
}
