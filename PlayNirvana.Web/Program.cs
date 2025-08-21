using System.Reflection;
using PlayNirvana.Web;
using PlayNirvana.CommonModule;
using PlayNirvana.RoundModule;
using PlayNirvana.TicketModule;

var builder = WebApplication.CreateBuilder(args);
builder.Logging
    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);

builder.Services.Configure<HostOptions>(hostOptions =>
{
    hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

//builder.Services.AddControllers().AddApplicationPart(Assembly.Load(new AssemblyName("PlayNirvana.RoundModule")));
builder.Services.AddControllers().AddApplicationPart(Assembly.Load(new AssemblyName("PlayNirvana.TicketModule")));
builder.Services.RegisterCommonModule();

builder.Services.RegisterWeb();
builder.Services.RegisterRoundModule(builder.Configuration);
builder.Services.RegisterTicketModule(builder.Configuration);

var app = builder.Build();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.RegisterRoundApps();
app.MapControllers();

app.Run();
