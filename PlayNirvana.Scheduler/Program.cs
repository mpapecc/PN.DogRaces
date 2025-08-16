using MassTransit;
using PlayNirvana.Bll.IoC;
using PlayNirvana.Scheduler.BackgroundServices;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<RoundStarterService>();
builder.Services.AddHostedService<RoundsGeneratorService>();
builder.Services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(30));
builder.Logging
    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);
builder.Services.RegisterBllModule();

builder.Services.AddMassTransit(x =>
{
    // No consumers here; this app only publishes
    x.SetKebabCaseEndpointNameFormatter();

    x.AddDelayedMessageScheduler();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.UseDelayedMessageScheduler();
    });
});

var host = builder.Build();
host.Run();

