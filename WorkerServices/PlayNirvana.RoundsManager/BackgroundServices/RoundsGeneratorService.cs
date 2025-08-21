using PlayNirvana.Bll.Services;

namespace PlayNirvana.RoundsManager.BackgroundServices
{
    public class RoundsGeneratorService : SchedulableBackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RoundsGeneratorService> logger;

        public RoundsGeneratorService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RoundsGeneratorService> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        //every day at midnight
        public override string CronExpression() => "*/30 * * * * *";

        public override Task JobAsync(CancellationToken ct)
        {
            try
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();
                var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();

                roundService.GenerateRoundIfNeeded();
            }
            catch (Exception e)
            {
                //raise critical error and notify since this is crutial service for app
                this.logger.LogCritical(e.Message);
            }

            return Task.CompletedTask;
        }
    }
}
