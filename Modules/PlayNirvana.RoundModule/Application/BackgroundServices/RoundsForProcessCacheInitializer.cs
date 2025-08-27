using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlayNirvana.RoundModule.Application.Services;

namespace PlayNirvana.RoundModule.Application.BackgroundServices
{
    public class RoundsForProcessCacheInitializer : BackgroundService
    {
        private readonly ILogger<RoundsForProcessCacheInitializer> logger;
        private readonly RoundsForProcessCache activeRoundCache;
        private readonly ScopeRunner scopeRunner;

        public RoundsForProcessCacheInitializer(
            ILogger<RoundsForProcessCacheInitializer> logger,
            RoundsForProcessCache activeRoundCache,
            ScopeRunner scopeRunner)
        {
            this.logger = logger;
            this.activeRoundCache = activeRoundCache;
            this.scopeRunner = scopeRunner;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            this.scopeRunner.Run<RoundService>(roundService =>
            {
                this.logger.LogInformation("{service} started", nameof(RoundsForProcessCacheInitializer));

                var activeRounds = roundService.GetRoundsForProcessDtos();

                foreach (var round in activeRounds)
                {
                    activeRoundCache.Enqueue(round);
                }

                this.logger.LogInformation("{service} finshed starting", nameof(RoundsForProcessCacheInitializer));
            });

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
