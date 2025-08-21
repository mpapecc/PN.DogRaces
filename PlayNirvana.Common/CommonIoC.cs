using Microsoft.Extensions.DependencyInjection;
using PlayNirvana.Common.Interfaces;

namespace PlayNirvana.Common
{
    public static class CommonIoC
    {
        public static IServiceCollection RegisterCommonModule(this IServiceCollection services)
        {
            return services;
        }
    }
}
