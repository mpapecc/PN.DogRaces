using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlayNirvana.CommonModule.Models;
using PlayNirvana.RoundModule.Application.Models;
using PlayNirvana.RoundModule.Application.Services;
using PlayNirvana.RoundModule.Domain.Entites;
using PlayNirvana.RoundModule.Integrations;
using PlayNirvana.RoundModule.Presentation.RoundHub;

namespace PlayNirvana.RoundModule.Application.BackgroundServices
{
    public class RoundManagerService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RoundManagerService> logger;
        private RoundModel? roundModel;

        public RoundManagerService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RoundManagerService> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();

            roundModel = roundService.GetNextActiveRoundModel();

            if (roundModel.Start <= DateTime.UtcNow)
            {
                roundService.TranslateActiveAndIdleRoundsStartInFuture();
                roundModel = roundService.GetNextActiveRoundModel();
            }

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            if (roundModel == null)
            {
                this.logger.LogCritical("There is not round for process!");
                return;
            }

            await Task.Delay(roundModel.Start - DateTime.UtcNow);
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(120));
            await ManageRoundAsync(true, ct);

            try
            {
                while (await timer.WaitForNextTickAsync(ct))
                {
                    await ManageRoundAsync(false, ct);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        private async Task ManageRoundAsync(bool isFirstExecution, CancellationToken ct)
        {
            if (roundModel == null)
            {
                this.logger.LogCritical("There is not round for process!");
                return;
            }

            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();
            var ticketModuleIntegration = scope.ServiceProvider.GetRequiredService<ITicketModuleIntegration>();
            var roundHub = scope.ServiceProvider.GetRequiredService<IHubContext<RoundHub, IRoundHubClient>>();

            var roundId = roundModel.Id;

            this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} started");
            await roundHub.Clients.All.RoundStarted();

            var raceStartDelay = Task.Delay(roundModel.CalculateUntilRaceStartWitBetLock());

            IEnumerable<RaceDogResultModel> roundOutcome = await raceStartDelay.ContinueWith((task) =>
            {
                this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} locked");

                roundService.LockRoundAsync(roundId);
                roundOutcome = roundService.GenerateRoundOutcome(roundId);

                this.logger.LogInformation($" {DateTime.UtcNow} : Race for round {roundId} started");

                roundHub.Clients.All.RaceStartWithBetLock(roundId);

                ticketModuleIntegration.ProcessRoundBets(new RoundBetsProcessData()
                {
                    RaceDogsResult = roundOutcome,
                    RoundId = roundId
                });

                return roundOutcome;
            }, ct);

            var roundFinishDelay = Task.Delay(roundModel.CalculateUntilRoundFinish());
            this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} finished");

            await roundFinishDelay.ContinueWith(async task =>
            {
                await roundService.FinishRoundAsync(roundModel.Id);
                await roundHub.Clients.All.RoundFinished(roundOutcome);
                roundModel = roundService.GetNextActiveRoundModel();
            });
        }
    }
}
