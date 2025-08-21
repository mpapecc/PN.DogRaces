using Microsoft.Extensions.DependencyInjection;

namespace PlayNirvana.CommonModule
{
    public static class CommonIoC
    {
        public static IServiceCollection RegisterCommonModule(this IServiceCollection services)
        {
            return services;
        }
    }
}
