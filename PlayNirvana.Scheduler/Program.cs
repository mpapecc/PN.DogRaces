using PlayNirvana.Bll.IoC;
using PlayNirvana.Scheduler.BackgroundServices.Implementation;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<RoundsGenerator>();
builder.Services.AddHostedService<RoundManager>();
builder.Services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(30));
builder.Logging
    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);
builder.Services.RegisterBllModule();

ConnectionFactory factory = new();
factory.HostName = "localhost";
factory.ClientProvidedName = "PlayNirvana.Scheduler";

IConnection connection = await factory.CreateConnectionAsync();
IChannel channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: "bets-processor",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null
    );

builder.Services.AddSingleton(channel);

var host = builder.Build();
host.Run();

