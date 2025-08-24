using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlayNirvana.RoundModule.Application.Models;
using PlayNirvana.RoundModule.Application.Services;
using PlayNirvana.RoundModule.Common.Enums;

namespace PlayNirvana.RoundModule.Application.BackgroundServices
{
    public class RoundsGeneratorService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RoundsGeneratorService> logger;
        private readonly ActiveRoundCache actieRoundCache;

        public RoundsGeneratorService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RoundsGeneratorService> logger,
            ActiveRoundCache actieRoundCache) 
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
            this.actieRoundCache = actieRoundCache;
        }


        protected override Task ExecuteAsync(CancellationToken ct)
        {
            GenerateRoundsJob();

            System.Timers.Timer timer = new System.Timers.Timer(TimeSpan.FromSeconds(10));

            timer.Elapsed += new System.Timers.ElapsedEventHandler(
                delegate { GenerateRoundsJob(); }
                );

            timer.Start();
            return Task.CompletedTask;
        }

        private void GenerateRoundsJob()
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();

            var newRounds = roundService.GenerateRoundIfNeeded();
            var activeRounds = newRounds.Where(x => x.RoundStatus == RoundStatus.Active);

            actieRoundCache.EnqueueList(activeRounds.Select(x => new RoundDto(x.Id, x.Start)));
        }
    }
}
