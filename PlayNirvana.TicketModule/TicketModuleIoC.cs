using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayNirvana.CommonModule;
using PlayNirvana.CommonModule.DataContext;
using PlayNirvana.CommonModule.Interfaces;
using PlayNirvana.TicketModule.Application.Repositories;
using PlayNirvana.TicketModule.Application.Services;
using PlayNirvana.TicketModule.External;
using PlayNirvana.TicketModule.Infrastructure.DataContext;
using PlayNirvana.TicketModule.Integrations;

namespace PlayNirvana.TicketModule
{
    public static class TicketModuleIoC
    {
        public static IServiceCollection RegisterTicketModule(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDbContext<TicketModuleDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PlayNirvanaConnectionString"), o =>
                {
                    o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, DbSchema.Tickets);
                });
            });

            services.AddScoped(typeof(ITicketModuleRepository<>), typeof(TicketModuleRepository<>));

            services.AddScoped<IRoundModuleIntegration, RoundModuleIntegration>();

            services.AddScoped<ITicketModuleExternal, TicketModuleExternal>();
            services.AddScoped<IPaymentModuleIntegration, PaymentModuleIntegration>();


            services.AddScoped<TicketService>();
            services.AddScoped<BetService>();

            services.AddValidators(typeof(TicketModuleIoC).Assembly);
            return services;
        }
    }
}
