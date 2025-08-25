using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlayNirvana.RoundModule.Application.Models;
using PlayNirvana.RoundModule.Application.Services;
using PlayNirvana.RoundModule.Common.Enums;
using PlayNirvana.RoundModule.Common.Options;

namespace PlayNirvana.RoundModule.Application.BackgroundServices
{
    public class RoundsGenerator : BackgroundService
    {
        private readonly ILogger<RoundsGenerator> logger;
        private readonly ActiveRoundCache actieRoundCache;
        private readonly ScopeRunner scopeRunner;
        private readonly RoundOptions roundOptions;

        public RoundsGenerator(
            ILogger<RoundsGenerator> logger,
            ActiveRoundCache actieRoundCache,
            IOptions<RoundOptions> roundOptions,
            ScopeRunner scopeRunner)
        {
            this.logger = logger;
            this.actieRoundCache = actieRoundCache;
            this.scopeRunner = scopeRunner;
            this.roundOptions = roundOptions.Value;
        }

        protected override Task ExecuteAsync(CancellationToken ct)
        {
            GenerateRoundsJob();

            System.Timers.Timer timer = new System.Timers.Timer(TimeSpan.FromMinutes(this.roundOptions.RoundsGeneratorIntervalInMinutes));

            timer.Elapsed += new System.Timers.ElapsedEventHandler(
                delegate { GenerateRoundsJob(); }
                );

            timer.Start();

            return Task.CompletedTask;
        }

        private void GenerateRoundsJob()
        {
            this.scopeRunner.Run<RoundsGeneratorService>(roundGeneratorService =>
            {
                if (roundGeneratorService.IsFirstRoundForProcessStartInFuture())
                {
                    roundGeneratorService.TranslateNonProcessedRoundsStartInFuture();
                }

                var newRounds = roundGeneratorService.GenerateRoundIfNeeded();
                var activeRounds = newRounds.Where(x => x.RoundStatus == RoundStatus.Active);

                actieRoundCache.EnqueueList(activeRounds.Select(x => new RoundDto(x.Id, x.Start)));
            });
        }
    }
}
