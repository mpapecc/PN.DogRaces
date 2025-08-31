using System.Text.Json.Serialization;

namespace PlayNirvana.Web
{
    public static class WebModule
    {
        public static IServiceCollection RegisterWeb(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddSignalR();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddExceptionHandler<ExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }
    }
}
