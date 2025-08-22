using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlayNirvana.RoundModule.Application.Models;
using PlayNirvana.RoundModule.Application.Services;
using PlayNirvana.RoundModule.Common.Exceptions;
using PlayNirvana.RoundModule.Integrations;
using PlayNirvana.RoundModule.Presentation.RoundHub;

namespace PlayNirvana.RoundModule.Application.BackgroundServices
{
    public class RoundManagerService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RoundManagerService> logger;
        private RoundDto roundModel { get; set; }
        private RoundDto nextRoundModel { get; set; }

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

            try
            {
                roundModel = roundService.GetNextActiveRoundModel();

                if (roundModel.IsStartInPast())
                {
                    roundService.TranslateActiveAndIdleRoundsStartInFuture();
                    roundModel = roundService.GetNextActiveRoundModel();
                }
            }
            catch (NoActiveRoundsException e)
            {
                this.logger.LogWarning(e.Message);

                roundService.GenerateRoundIfNeeded();
                roundModel = roundService.GetNextActiveRoundModel();
            }
            catch (Exception e)
            {
                this.logger.LogError("Unandled exception {error}",e);
                throw;
            }

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            try
            {
                await Task.Delay(roundModel.CalculateUntilStart());
                using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(120));
                await ManageRoundAsync(true, ct);

                while (await timer.WaitForNextTickAsync(ct))
                {
                    await ManageRoundAsync(false, ct);
                }
            }
            catch (Exception e)
            {
                this.logger.LogError("Unandled exception {error}", e);
                this.logger.LogError("Starting {service} again!", nameof(RoundManagerService));
                await StartAsync(ct);
            }
        }

        private async Task ManageRoundAsync(bool isFirstExecution, CancellationToken ct)
        {

            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();
            var ticketModuleIntegration = scope.ServiceProvider.GetRequiredService<ITicketModuleIntegration>();
            var roundHub = scope.ServiceProvider.GetRequiredService<IHubContext<RoundHub, IRoundHubClient>>();

            var roundId = roundModel.Id;

            this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} started");
            roundHub.Clients.All.RoundStarted();

            var raceStartDelay = Task.Delay(roundModel.CalculateUntilRaceStartWitBetLock());

            await raceStartDelay.ContinueWith((task) =>
            {
                this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} locked");

                roundService.LockRound(roundId);

                roundHub.Clients.All.RaceStartWithBetLock(roundId);

                var outcome = roundService.GenerateRoundOutcome(roundId);

                this.logger.LogInformation($" {DateTime.UtcNow} : Race for round {roundId} started");
                //this can be done async since its long runnig process and it does not depends on anything
                //or even better offload it to hangfire or something
                ticketModuleIntegration.ProcessRoundBets(roundId);

                return outcome;
            }, ct);

            var roundFinishDelay = Task.Delay(roundModel.CalculateUntilRoundFinish());

            await roundFinishDelay.ContinueWith(task =>
            {
                this.logger.LogInformation($" {DateTime.UtcNow} : Race for round {roundId} finished");
                roundService.FinishRound(roundModel.Id);
                this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} finished");
            },ct);

            roundModel = roundService.GetNextActiveRoundModel();
        }
    }
}
