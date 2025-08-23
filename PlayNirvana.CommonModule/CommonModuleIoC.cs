using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PlayNirvana.CommonModule.Services;

namespace PlayNirvana.CommonModule
{
    public static class CommonModuleIoC
    {
        public static IServiceCollection RegisterCommonModule(this IServiceCollection services)
        {
            services.AddScoped<IExecuteUpdateOrDeleteBatcher, ExecuteUpdateOrDeleteBatcher>();

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
