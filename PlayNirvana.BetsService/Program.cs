using PlayNirvana.Bll.IoC;
using MassTransit;
using PlayNirvana.Shared.Options;
using Microsoft.IdentityModel.Protocols.Configuration;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.RegisterBllModule();
builder.Logging
    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);

var massTransitOptions = builder.Configuration.GetSection(nameof(MassTransitOptions)).Get<MassTransitOptions>();

if (massTransitOptions == null)
    throw new InvalidConfigurationException("MassTransit configuration is missing.");

builder.Services.AddMassTransit(x =>
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

        cfg.ConfigureEndpoints(context);
    });
});


var host = builder.Build();
host.Run();
