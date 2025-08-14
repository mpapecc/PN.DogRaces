using PlayNirvana.Bll.IoC;
using PlayNirvana.BetsService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.RegisterBllModule();
builder.Services.AddHostedService<BetsServiceWorker>();
//builder.Logging
//    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);

var host = builder.Build();
host.Run();
