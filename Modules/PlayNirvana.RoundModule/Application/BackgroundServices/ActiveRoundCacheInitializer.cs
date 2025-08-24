using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlayNirvana.RoundModule.Application.Services;

namespace PlayNirvana.RoundModule.Application.BackgroundServices
{
    public class ActiveRoundCacheInitializer : BackgroundService
    {
        private readonly ILogger<ActiveRoundCacheInitializer> logger;
        private readonly ActiveRoundCache activeRoundCache;
        private readonly ScopeRunner scopeRunner;

        public ActiveRoundCacheInitializer(
            ILogger<ActiveRoundCacheInitializer> logger,
            ActiveRoundCache activeRoundCache,
            ScopeRunner scopeRunner)
        {
            this.logger = logger;
            this.activeRoundCache = activeRoundCache;
            this.scopeRunner = scopeRunner;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("{service} started", nameof(ActiveRoundCacheInitializer));

            this.scopeRunner.Run<RoundService>(roundService =>
            {
                var activeRounds = roundService.GetActiveRoundDtos();

                foreach (var round in activeRounds)
                {
                    activeRoundCache.Enqueue(round);
                }

                this.logger.LogInformation("{service} finshed starting", nameof(ActiveRoundCacheInitializer));
            });

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
