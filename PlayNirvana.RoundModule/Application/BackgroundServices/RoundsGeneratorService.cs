using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlayNirvana.RoundModule.Application.Services;

namespace PlayNirvana.RoundModule.Application.BackgroundServices
{
    public class RoundsGeneratorService : SchedulableBackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RoundsGeneratorService> logger;

        public RoundsGeneratorService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RoundsGeneratorService> logger) : base(logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        } 

        public override string CronExpression() => "0 0 * * * *";

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
                logger.LogError(e, "RoundsGenerator failure!");
                base.StartAsync(ct);
            }

            return Task.CompletedTask;
        }
    }
}
