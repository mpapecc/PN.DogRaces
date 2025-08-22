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
    }
}
