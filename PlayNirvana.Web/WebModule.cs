using MassTransit;
using Microsoft.IdentityModel.Protocols.Configuration;
using PlayNirvana.Common;
using PlayNirvana.RoundModule;
using PlayNirvana.TicketModule;

//using PlayNirvana.Web.Consumers;

namespace PlayNirvana.Web
{
    public static class WebModule
    {
        public static IServiceCollection RegisterWeb(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddExceptionHandler<ExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }
    }
}
