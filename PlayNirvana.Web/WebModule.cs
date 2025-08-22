namespace PlayNirvana.Web
{
    public static class WebModule
    {
        public static IServiceCollection RegisterWeb(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddSignalR();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddExceptionHandler<ExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }
    }
}
