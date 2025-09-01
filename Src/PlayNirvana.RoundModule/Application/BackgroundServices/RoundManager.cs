using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlayNirvana.RoundModule.Application.Models;
using PlayNirvana.RoundModule.Application.Repositories;
using PlayNirvana.RoundModule.Application.Services;
using PlayNirvana.RoundModule.Common.Options;
using PlayNirvana.RoundModule.Domain.Entites;
using PlayNirvana.RoundModule.Integrations;
using PlayNirvana.RoundModule.Presentation.RoundHub;

namespace PlayNirvana.RoundModule.Application.BackgroundServices
{
    public class RoundManager : BackgroundService
    {
        private readonly RoundOptions roundOptions;
        private readonly ILogger<RoundManager> logger;
        private readonly RoundsForProcessCache activeRoundCache;
        private readonly ScopeRunner scopeRunner;

        public RoundManager(
            IOptions<RoundOptions> roundOptions,
            ILogger<RoundManager> logger,
            RoundsForProcessCache activeRoundCache,
            ScopeRunner scopeRunner)
        {
            this.roundOptions = roundOptions.Value;
            this.logger = logger;
            this.activeRoundCache = activeRoundCache;
            this.scopeRunner = scopeRunner;
        }

        public override Task StartAsync(CancellationToken ct)
        {
            //this is case when round is locked and then system crashes
            //we want on next startup detect if there are locked rounds and process them first
            Task.Run(() => ProcessLockedRoundsBetsIfNeeded(), ct);
            return base.StartAsync(ct);
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            try
            {
                var roundModel = this.activeRoundCache.Peek();
                await Task.Delay(roundModel.CalculateUntilRoundStart());

                using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(this.roundOptions.RoundDurationInSeconds));
                await ManageRoundAsync(this.activeRoundCache.Dequeue());

                while (await timer.WaitForNextTickAsync(ct))
                {
                    if (this.activeRoundCache.Any())
                    {
                        await ManageRoundAsync(this.activeRoundCache.Dequeue());
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

        private void ProcessLockedRoundsBetsIfNeeded()
        {
            this.scopeRunner.Run<IRoundRepository, ITicketModuleIntegration, IRoundModuleRepository<RaceDogResult>, RoundOutcomeService, RoundService>(
                (roundRepository, ticketModuleIntegration, raceDogResultRepository, roundOutcomeService, roundService) =>
            {
                var lockedRoundsIds = roundRepository.LockedRoundQuery().Select(x => x.Id).ToList();

                if (!lockedRoundsIds.Any())
                {
                    return;
                }

                lockedRoundsIds.ForEach((roundId) =>
                {
                    if(!raceDogResultRepository.Query().Any(x => x.RoundId == roundId))
                    {
                        roundOutcomeService.GenerateRoundOutcome(roundId);
                    }

                    ticketModuleIntegration.ProcessRoundBets(roundId);
                    roundService.FinishRound(roundId);
                });
            });
        }

        private Task ManageRoundAsync(RoundDto roundModel)
        {
            var roundId = roundModel.Id;

            this.scopeRunner.Run<RoundService, IHubContext<RoundHub, IRoundHubClient>>((roundService, roundHub) =>
            {
                roundService.StartRoundProgress(roundId);
                roundHub.Clients.All.RoundStarted(roundId);
            });


            this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} started");

            System.Timers.Timer raceStartTimer = new System.Timers.Timer(roundModel.CalculateUntilRaceStartWitBetLock(this.roundOptions.DurationFromRoundStartToRaceStart()))
            {
                AutoReset = false
            };

            raceStartTimer.Elapsed += new System.Timers.ElapsedEventHandler(delegate { OnRaceStartEvent(roundId); });
            raceStartTimer.Start();

            System.Timers.Timer raceFinishTimer = new System.Timers.Timer(roundModel.CalculateUntilRoundFinish(this.roundOptions.RoundDurationInSeconds))
            {
                AutoReset = false
            };

            raceFinishTimer.Elapsed += new System.Timers.ElapsedEventHandler(delegate { OnRoundFinishEvent(roundId); });
            raceFinishTimer.Start();

            return Task.CompletedTask;
        }

        private void OnRaceStartEvent(int roundId)
        {
            try
            {
                this.scopeRunner.Run<RoundService, RoundOutcomeService, IHubContext<RoundHub, IRoundHubClient>>(
                    (roundService, roundOutcomeService, roundHub) =>
                {
                    this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} locked");

                    roundService.LockRound(roundId);

                    roundHub.Clients.All.RaceStartWithBetLock(roundId);

                    var outcome = roundOutcomeService.GenerateRoundOutcome(roundId);

                    this.logger.LogInformation($" {DateTime.UtcNow} : Race for round {roundId} started");

                    //this can be done async since its long runnig process and it does not depends on anything
                    //or even better offload it to hangfire or something
                    Task.Run(() =>
                    {
                        this.scopeRunner.Run<ITicketModuleIntegration>(ticketModuleIntegration =>
                        {

                            ticketModuleIntegration.ProcessRoundBets(roundId);
                        });
                    });
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
                this.scopeRunner.Run<RoundService>((roundService) =>
                {
                    this.logger.LogInformation($" {DateTime.UtcNow} : Race for round {roundId} finished");
                    roundService.FinishRound(roundId);
                    this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} finished");
                });
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception in {name} event handler for RoundId {roundId}: : {error}", 
                    nameof(OnRoundFinishEvent), roundId, e);
            }
        }
    }
}
