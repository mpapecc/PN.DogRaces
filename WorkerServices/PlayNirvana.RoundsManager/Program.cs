using MassTransit;
using Microsoft.IdentityModel.Protocols.Configuration;
using PlayNirvana.Bll;
using PlayNirvana.Bll.Services;
using PlayNirvana.Infrastructure;
using PlayNirvana.RoundsManager.BackgroundServices;
using PlayNirvana.Scheduler.BackgroundServices;
using PlayNirvana.Shared.Options;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<RoundManagerService>();
builder.Services.AddHostedService<RoundsGeneratorService>();
builder.Services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(30));
builder.Logging
    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);
builder.Services.RegisterBllModule();
builder.Services.RegisterInfrastructureModule();

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

//handle db validation checks related to Rounds
using IServiceScope scope = host.Services.GetService<IServiceScopeFactory>().CreateScope();
var roundService = scope.ServiceProvider.GetService<RoundService>();

roundService.TranslateActiveAndIdleRoundsStartInFuture();
roundService.GenerateRoundIfNeeded();

host.Run();

