using PlayNirvana.Bll.IoC;
using MassTransit;
using PlayNirvana.BetsService.Consumers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.RegisterBllModule();
//builder.Logging
//    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<RoundsForProcessConsumer>();

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


var host = builder.Build();
host.Run();
