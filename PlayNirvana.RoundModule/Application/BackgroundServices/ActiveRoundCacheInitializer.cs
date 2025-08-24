using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlayNirvana.RoundModule.Application.Services;

namespace PlayNirvana.RoundModule.Application.BackgroundServices
{
    public class ActiveRoundCacheInitializer : BackgroundService
    {
        private readonly ILogger<ActiveRoundCacheInitializer> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ActiveRoundCache activeRoundCache;

        public ActiveRoundCacheInitializer(
            ILogger<ActiveRoundCacheInitializer> logger,
            IServiceScopeFactory serviceScopeFactory,
            ActiveRoundCache activeRoundCache)
        {
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
            this.activeRoundCache = activeRoundCache;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("{service} started", nameof(ActiveRoundCacheInitializer));

            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();

            var activeRounds = roundService.GetActiveRoundDtos();

            foreach (var round in activeRounds)
            {
                activeRoundCache.Enqueue(round);
            }

            this.logger.LogInformation("{service} finshed starting", nameof(ActiveRoundCacheInitializer));

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
