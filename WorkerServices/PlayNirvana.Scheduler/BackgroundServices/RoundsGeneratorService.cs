using PlayNirvana.Bll.Services;

namespace PlayNirvana.Scheduler.BackgroundServices
{
    public class RoundsGeneratorService : SchedulableBackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public RoundsGeneratorService(
            IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        //every day at midnight
        public override string CronExpression() => "*/30 * * * * *";

        public override Task JobAsync(CancellationToken ct)
        {
            return GeneratRounds();
        }

        private Task GeneratRounds()
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();

            return roundService.GenerateRoundIfNeeded();
        }
    }
}
