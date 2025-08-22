using Microsoft.Extensions.DependencyInjection;
using PlayNirvana.CommonModule.Interfaces;
using PlayNirvana.PaymentModule.Application;
using PlayNirvana.PaymentModule.External;

namespace PlayNirvana.PaymentModule
{
    public static class PaymentModuleIoc
    {
        public static IServiceCollection RegisterPaymentModule(this IServiceCollection services)
        {
            services.AddScoped<PaymentService>();
            services.AddScoped<IPaymentModuleExternal, PaymentModuleExternal>();

            return services;
        }
    }
}
