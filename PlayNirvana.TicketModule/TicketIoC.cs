using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayNirvana.CommonModule.Interfaces;
using PlayNirvana.TicketModule.Application.Repositories;
using PlayNirvana.TicketModule.Application.Services;
using PlayNirvana.TicketModule.Application.Validators;
using PlayNirvana.TicketModule.External;
using PlayNirvana.TicketModule.Infrastructure.DataContext;
using PlayNirvana.TicketModule.Integrations;

namespace PlayNirvana.TicketModule
{
    public static class TicketIoC
    {
        public static IServiceCollection RegisterTicketModule(this IServiceCollection services, IConfigurationManager configuration)
        {

            services.AddDbContext<TicketModuleDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PlayNirvanaConnectionString"), o =>
                {
                    o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, TicketModuleDbContext.schema);
                });
            });

            services.AddScoped(typeof(ITicketModuleRepository<>), typeof(TicketModuleRepository<>));

            services.AddScoped<IRoundModuleIntegration, RoundModuleIntegration>();
            services.AddScoped<ITicketModuleExternal, TicketModuleExternal>();

            services.AddScoped<TicketService>();
            services.AddScoped<BetService>();
            services.AddValidators(typeof(TicketIoC).Assembly);
            return services;
        }

        private static IServiceCollection AddValidators(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddScoped(typeof(Validator<>));

            var validatorInterfaceType = typeof(IValidator<>);

            var types = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsInterface &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == validatorInterfaceType))
                .GroupBy(t => t.GetInterfaces().First().GetGenericArguments().First())
                .ToList();

            foreach (var t in types)
            {
                var genericInterfaceType = validatorInterfaceType.MakeGenericType(t.Key);

                foreach (var serviceType in t)
                {
                    services.Add(new ServiceDescriptor(genericInterfaceType, serviceType, ServiceLifetime.Scoped));
                    services.Add(new ServiceDescriptor(serviceType, serviceType, ServiceLifetime.Scoped));
                }
            }

            return services;
        }
    }
}
