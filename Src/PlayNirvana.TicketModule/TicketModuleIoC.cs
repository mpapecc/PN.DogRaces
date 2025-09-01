using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlayNirvana.CommonModule;
using PlayNirvana.CommonModule.DataContext;
using PlayNirvana.CommonModule.Interfaces;
using PlayNirvana.TicketModule.Application.Repositories;
using PlayNirvana.TicketModule.Application.Services;
using PlayNirvana.TicketModule.Common.Options;
using PlayNirvana.TicketModule.External;
using PlayNirvana.TicketModule.Infrastructure.DataContext;
using PlayNirvana.TicketModule.Integrations;

namespace PlayNirvana.TicketModule
{
    public static class TicketModuleIoC
    {
        public static IServiceCollection RegisterTicketModule(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.RegisterInfrastructure(configuration)
                .RegisterServices()
                .RegisterOptions(configuration);

            return services;
        }

        private static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDbContext<TicketModuleDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PlayNirvanaConnectionString"), o =>
                {
                    o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, DbSchema.Tickets);
                });
            });

            services.AddScoped(typeof(ITicketModuleRepository<>), typeof(TicketModuleRepository<>));

            services.AddScoped<IPaymentModuleIntegration, PaymentModuleIntegration>();
            services.AddScoped<IRoundModuleIntegration, RoundModuleIntegration>();

            return services;
        }

        private static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<TicketService>();
            services.AddScoped<BetService>();
            services.AddScoped<ITicketModuleExternal, TicketModuleExternal>();
            services.AddValidators(typeof(TicketModuleIoC).Assembly);

            return services;
        }

        private static IServiceCollection RegisterOptions(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.Configure<TicketOptions>(configuration.GetSection(nameof(TicketOptions)));
            return services;
        }
    }
}
