using Microsoft.Extensions.DependencyInjection;

namespace PlayNirvana.RoundModule.Application.BackgroundServices
{
    public class ScopeRunner
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public ScopeRunner(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void Run<S1>(Action<S1> action) 
            where S1 : notnull
        {
            using var scope = serviceScopeFactory.CreateScope();
            var service1 = scope.ServiceProvider.GetRequiredService<S1>();
            action(service1);
        }

        public void Run<S1, S2>(Action<S1, S2> action)
            where S1 : notnull 
            where S2 : notnull
        {
            using var scope = serviceScopeFactory.CreateScope();
            var service1 = scope.ServiceProvider.GetRequiredService<S1>();
            var service2 = scope.ServiceProvider.GetRequiredService<S2>();
            action(service1, service2);
        }

        public void Run<S1, S2, S3>(Action<S1, S2, S3> action)
            where S1 : notnull
            where S2 : notnull
            where S3 : notnull
        {
            using var scope = serviceScopeFactory.CreateScope();
            var service1 = scope.ServiceProvider.GetRequiredService<S1>();
            var service2 = scope.ServiceProvider.GetRequiredService<S2>();
            var service3 = scope.ServiceProvider.GetRequiredService<S3>();
            action(service1, service2, service3);
        }

        public void Run<S1, S2, S3, S4>(Action<S1, S2, S3, S4> action)
            where S1 : notnull
            where S2 : notnull
            where S3 : notnull
            where S4 : notnull
        {
            using var scope = serviceScopeFactory.CreateScope();
            var service1 = scope.ServiceProvider.GetRequiredService<S1>();
            var service2 = scope.ServiceProvider.GetRequiredService<S2>();
            var service3 = scope.ServiceProvider.GetRequiredService<S3>();
            var service4 = scope.ServiceProvider.GetRequiredService<S4>();
            action(service1, service2, service3, service4);
        }
    }
}
