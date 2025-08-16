using MassTransit;
using PlayNirvana.Bll.IoC;
using PlayNirvana.Web.Consumers;

namespace PlayNirvana.Web.IoC
{
    public static class WebModule
    {
        public static IServiceCollection RegisterWebModule(this IServiceCollection services)
        {
            services.RegisterBllModule();

            services.AddSignalR();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddExceptionHandler<ExceptionHandler>();
            services.AddProblemDetails();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<RoundsFinishedConsumer>();

                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
