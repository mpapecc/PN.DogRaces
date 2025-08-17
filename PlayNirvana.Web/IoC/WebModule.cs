using MassTransit;
using Microsoft.IdentityModel.Protocols.Configuration;
using PlayNirvana.Bll.IoC;
using PlayNirvana.Shared.Options;
using PlayNirvana.Web.Consumers;

namespace PlayNirvana.Web.IoC
{
    public static class WebModule
    {
        public static IServiceCollection RegisterWebModule(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.RegisterBllModule();
            services.RegisterMassTransit(configuration);

            services.AddSignalR();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddExceptionHandler<ExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }

        private static void RegisterMassTransit(this IServiceCollection services, IConfigurationManager configuration)
        {
            var massTransitOptions = configuration.GetSection(nameof(MassTransitOptions)).Get<MassTransitOptions>();

            if (massTransitOptions == null)
                throw new InvalidConfigurationException("MassTransit configuration is missing.");

            services.AddMassTransit(x =>
            {
                x.AddConsumers(typeof(Program).Assembly);

                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(massTransitOptions.Host, h =>
                    {
                        h.Username(massTransitOptions.User);
                        h.Password(massTransitOptions.Pwd);
                    });

                    cfg.ReceiveEndpoint(new TemporaryEndpointDefinition(configureConsumeTopology: true), SnakeCaseEndpointNameFormatter.Instance, e =>
                    {
                        e.ConfigureConsumer<RoundStartedConsumer>(context);
                        e.ConfigureConsumer<RoundFinishedConsumer>(context);

                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
