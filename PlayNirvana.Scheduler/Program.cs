using MassTransit;
using PlayNirvana.Bll.IoC;
using PlayNirvana.Scheduler.BackgroundServices.Implementation;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<RoundsGenerator>();
builder.Services.AddHostedService<RoundManager>();
builder.Services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(30));
builder.Logging
    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);
builder.Services.RegisterBllModule();

builder.Services.AddMassTransit(x =>
{
    // No consumers here; this app only publishes
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        // no ConfigureEndpoints() needed (no receive endpoints)
    });
});

var host = builder.Build();
host.Run();

