using MassTransit;
using PlayNirvana.Web.Consumers;
using PlayNirvana.Web.GameHubs;
using PlayNirvana.Web.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterWebModule();
builder.Services.AddSignalR();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<RoundsFinishedConsumer>();

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
