namespace PlayNirvana.Web
{
    public interface ITestDatabaseMigrator
    {
        void Migrate();
    }

    public static class TestDatabaseMigrator
    {
        public static void ApplyTestDatabaseMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            scope.ServiceProvider.GetService<ITestDatabaseMigrator>()?.Migrate();
        }
    }
}
