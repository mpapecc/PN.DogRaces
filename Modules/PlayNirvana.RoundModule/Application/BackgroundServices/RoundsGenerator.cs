using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RoundsGenerator> logger;
        private readonly ActiveRoundCache actieRoundCache;
        private readonly RoundOptions roundOptions;

        public RoundsGenerator(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RoundsGenerator> logger,
            ActiveRoundCache actieRoundCache,
            IOptions<RoundOptions> roundOptions) 
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
            this.actieRoundCache = actieRoundCache;
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
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundGeneratorService = scope.ServiceProvider.GetRequiredService<RoundsGeneratorService>();

            var newRounds = roundGeneratorService.GenerateRoundIfNeeded();
            var activeRounds = newRounds.Where(x => x.RoundStatus == RoundStatus.Active);

            actieRoundCache.EnqueueList(activeRounds.Select(x => new RoundDto(x.Id, x.Start)));
        }
    }
}
