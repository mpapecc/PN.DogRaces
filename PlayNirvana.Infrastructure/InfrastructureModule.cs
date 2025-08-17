using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlayNirvana.Infrastructure.DataContext;
using PlayNirvana.Bll.Repositories;
using PlayNirvana.Infrastructure.DataContext.Repositories;

namespace PlayNirvana.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection RegisterInfrastructureModule(this IServiceCollection services)
        {
            services.AddDbContext<PlayNirvanaDbContext>(options =>
            {
                options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;MultipleActiveResultSets=True;Initial Catalog=PlayNirvana;Application Name=PlayNirvana");
            });

            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IRoundRepository, RoundRepository>();

            return services;
        } 
    }
}
