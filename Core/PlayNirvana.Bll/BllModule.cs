using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PlayNirvana.Bll.Services;
using PlayNirvana.Bll.Validators;

namespace PlayNirvana.Bll
{
    public static class BllModule
    {
        public static IServiceCollection RegisterBllModule(this IServiceCollection services)
        {
            services.AddScoped<RoundService>();
            services.AddScoped<TicketService>();
            services.AddScoped<BetService>();
            services.AddSingleton<WalletService>();

            services.AddValidators(typeof(BllModule).Assembly);

            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services, params Assembly[] assemblies)
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
