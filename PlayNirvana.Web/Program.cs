using PlayNirvana.Web;
using PlayNirvana.CommonModule;
using PlayNirvana.RoundModule;
using PlayNirvana.TicketModule;
using PlayNirvana.PaymentModule;

var builder = WebApplication.CreateBuilder(args);

//builder.Logging
//    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);

builder.Services.RegisterCommonModule();

builder.Services.RegisterWeb();
builder.Services.RegisterRoundModule(builder.Configuration);
builder.Services.RegisterTicketModule(builder.Configuration);
builder.Services.RegisterPaymentModule();

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
