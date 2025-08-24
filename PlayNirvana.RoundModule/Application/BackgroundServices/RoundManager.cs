using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlayNirvana.RoundModule.Application.Models;
using PlayNirvana.RoundModule.Application.Services;
using PlayNirvana.RoundModule.Common.Options;
using PlayNirvana.RoundModule.Integrations;
using PlayNirvana.RoundModule.Presentation.RoundHub;

namespace PlayNirvana.RoundModule.Application.BackgroundServices
{
    public class RoundManager : BackgroundService
    {
        private readonly RoundOptions roundOptions;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RoundManager> logger;
        private readonly ActiveRoundCache activeRoundCache;
        //private RoundDto roundModel;

        public RoundManager(
            IOptions<RoundOptions> roundOptions,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RoundManager> logger,
            ActiveRoundCache activeRoundCache)
        {
            this.roundOptions = roundOptions.Value;
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
            this.activeRoundCache = activeRoundCache;
        }

        //public override Task StartAsync(CancellationToken cancellationToken)
        //{
        //    this.logger.LogInformation("{service} started", nameof(RoundManagerService));

        //    using IServiceScope scope = serviceScopeFactory.CreateScope();
        //    var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();

        //    try
        //    {
        //        //get in memory cache
        //        roundModel = roundService.GetNextActiveRoundModel();

        //        if (roundModel.IsStartInPast())
        //        {
        //            roundService.TranslateActiveAndIdleRoundsStartInFuture();
        //            roundModel = roundService.GetNextActiveRoundModel();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        this.logger.LogError("Unandled exception {error}",e);
        //        throw;
        //    }
        //    this.logger.LogInformation("{service} finished starting", nameof(RoundManagerService));

        //    return base.StartAsync(cancellationToken);
        //}

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            try
            {
                var roundModel = this.activeRoundCache.Peek();

                await Task.Delay(roundModel.CalculateUntilStart());
                using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(this.roundOptions.RoundDurationInSeconds));
                await ManageRoundAsync(this.activeRoundCache.Dequeue(), ct);

                while (await timer.WaitForNextTickAsync(ct))
                {
                    if (this.activeRoundCache.Any())
                    {
                        await ManageRoundAsync(this.activeRoundCache.Dequeue(), ct);
                    }
                    else
                    {
                        this.logger.LogCritical("No more active rounds for processing !!!");
                    }
                }
            }
            catch (Exception e)
            {
                this.logger.LogError("Unandled exception {error}", e);
                this.logger.LogError("Starting {service} again!", nameof(RoundManager));
                await StartAsync(ct);
            }
        }

        private async Task ManageRoundAsync(RoundDto roundModel, CancellationToken ct)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();
            var ticketModuleIntegration = scope.ServiceProvider.GetRequiredService<ITicketModuleIntegration>();
            var roundHub = scope.ServiceProvider.GetRequiredService<IHubContext<RoundHub, IRoundHubClient>>();

            var roundId = roundModel.Id;

            this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} started");
            await roundHub.Clients.All.RoundStarted(roundId);

            System.Timers.Timer raceStartTimer = new System.Timers.Timer(roundModel.CalculateUntilRaceStartWitBetLock())
            {
                AutoReset = false
            };

            raceStartTimer.Elapsed += new System.Timers.ElapsedEventHandler(delegate { OnRaceStartEvent(roundId); });
            raceStartTimer.Start();

            System.Timers.Timer raceFinishTimer = new System.Timers.Timer(roundModel.CalculateUntilRoundFinish())
            {
                AutoReset = false
            };

            raceFinishTimer.Elapsed += new System.Timers.ElapsedEventHandler(delegate { OnRoundFinishEvent(roundId); });
            raceFinishTimer.Start();

        }

        private void OnRaceStartEvent(int roundId)
        {
            try
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();
                var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();
                var roundOutcomeService = scope.ServiceProvider.GetRequiredService<RoundOutcomeService>();
                var roundHub = scope.ServiceProvider.GetRequiredService<IHubContext<RoundHub, IRoundHubClient>>();

                this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} locked");

                roundService.LockRound(roundId);

                roundHub.Clients.All.RaceStartWithBetLock(roundId);

                var outcome = roundOutcomeService.GenerateRoundOutcome(roundId);

                this.logger.LogInformation($" {DateTime.UtcNow} : Race for round {roundId} started");

                //this can be done async since its long runnig process and it does not depends on anything
                //or even better offload it to hangfire or something
                Task.Run(() =>
                {
                    using IServiceScope scope = serviceScopeFactory.CreateScope();
                    var ticketModuleIntegration = scope.ServiceProvider.GetRequiredService<ITicketModuleIntegration>();
                    ticketModuleIntegration.ProcessRoundBets(roundId);
                });
            }
            catch (Exception e)
            {
                this.logger.LogError(
                    "Exception in {name} event handler for RoundId {roundId} : {error}", 
                    nameof(OnRaceStartEvent), e, roundId);
            }
        }

        private void OnRoundFinishEvent(int roundId)
        {
            try
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();
                var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();

                this.logger.LogInformation($" {DateTime.UtcNow} : Race for round {roundId} finished");
                roundService.FinishRound(roundId);
                this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} finished");
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception in {name} event handler for RoundId {roundId}: : {error}", 
                    nameof(OnRoundFinishEvent), roundId, e);
            }
        }
    }
}
