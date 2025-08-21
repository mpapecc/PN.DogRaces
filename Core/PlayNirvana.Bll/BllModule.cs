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

            //services.AddValidators(typeof(BllModule).Assembly);

            return services;
        }

        
    }
}
