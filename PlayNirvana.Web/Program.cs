using MassTransit;
using PlayNirvana.Web.Consumers;
using PlayNirvana.Web.GameHubs;
using PlayNirvana.Web.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterWebModule();
builder.Logging
    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);

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
app.MapHub<GameHub>("/gamehub");
app.MapControllers();

app.Run();
