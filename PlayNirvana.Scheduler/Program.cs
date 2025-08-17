using MassTransit;
using Microsoft.IdentityModel.Protocols.Configuration;
using PlayNirvana.Bll.IoC;
using PlayNirvana.Scheduler.BackgroundServices;
using PlayNirvana.Shared.Options;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<RoundStarterService>();
builder.Services.AddHostedService<RoundsGeneratorService>();
builder.Services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(30));
builder.Logging
    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);
builder.Services.RegisterBllModule();

var massTransitOptions = builder.Configuration.GetSection(nameof(MassTransitOptions)).Get<MassTransitOptions>();

if (massTransitOptions == null)
    throw new InvalidConfigurationException("MassTransit configuration is missing.");

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddDelayedMessageScheduler();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(massTransitOptions.Host, h =>
        {
            h.Username(massTransitOptions.User);
            h.Password(massTransitOptions.Pwd);
        });

        cfg.UseDelayedMessageScheduler();
    });
});

var host = builder.Build();
host.Run();

