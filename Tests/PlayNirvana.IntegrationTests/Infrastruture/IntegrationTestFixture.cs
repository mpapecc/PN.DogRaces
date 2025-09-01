using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PlayNirvana.RoundModule.Application.BackgroundServices;

namespace PlayNirvana.IntegrationTests.Infrastruture
{
    public abstract class IntegrationTestFixture : IClassFixture<IntegrationTestWepAppFactory>
    {
        protected readonly WebApplicationFactory<Program> factory;
        protected readonly ScopeRunner scopeRunner;

        public IntegrationTestFixture(IntegrationTestWepAppFactory factory)
        {
            this.factory = factory;
            scopeRunner = factory.Services.GetRequiredService<ScopeRunner>();
        }
    }
}
