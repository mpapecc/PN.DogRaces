using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlayNirvana.RoundModule.Application.BackgroundServices;
using PlayNirvana.RoundModule.Infrastructure.DataContext;
using PlayNirvana.TicketModule.Infrastructure.DataContext;
using Testcontainers.MsSql;
using PlayNirvana.Web;

namespace PlayNirvana.IntegrationTests.Infrastruture
{
    public class IntegrationTestWepAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer sqlServerContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithName("sql-server-test")
            .WithPassword("Welcome$2u")
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<TicketModuleDbContext>>();
                services.RemoveAll<DbContextOptions<RoundModuleDbContext>>();
                services.RemoveAll<ITestDatabaseMigrator>();

                services.AddDbContext<TicketModuleDbContext>(o =>
                {
                    o.UseSqlServer(sqlServerContainer.GetConnectionString());
                });

                services.AddDbContext<RoundModuleDbContext>(o =>
                {
                    o.UseSqlServer(sqlServerContainer.GetConnectionString());
                });

                services.AddSingleton<ITestDatabaseMigrator, TestMigrator>();
            });
        }

        public Task InitializeAsync()
        {
            return this.sqlServerContainer.StartAsync();
        }

        public new Task DisposeAsync()
        {
            return this.sqlServerContainer.StopAsync();
        }
    }

    public class TestMigrator : ITestDatabaseMigrator
    {
        public TestMigrator(ScopeRunner scopeRunner)
        {
            ScopeRunner = scopeRunner;
        }

        public ScopeRunner ScopeRunner { get; }

        public void Migrate()
        {
            this.ScopeRunner.Run<RoundModuleDbContext, TicketModuleDbContext>(
                (ticketDb, roundDb) =>
                {
                    ticketDb.Database.Migrate();
                    roundDb.Database.Migrate();
                });
        }
    }
}
