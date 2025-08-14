using PlayNirvana.Bll.IoC;

namespace PlayNirvana.Web.IoC
{
    public static class WebModule
    {
        public static IServiceCollection RegisterWebModule(this IServiceCollection services)
        {
            services.RegisterBllModule();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddExceptionHandler<ExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }
    }
}
