using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlayNirvana.RoundModule.Application.Models;
using PlayNirvana.RoundModule.Application.Services;
using PlayNirvana.RoundModule.Common.Exceptions;
using PlayNirvana.RoundModule.Common.Options;
using PlayNirvana.RoundModule.Integrations;
using PlayNirvana.RoundModule.Presentation.RoundHub;

namespace PlayNirvana.RoundModule.Application.BackgroundServices
{
    public class RoundManagerService : BackgroundService
    {
        private readonly RoundOptions roundOptions;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RoundManagerService> logger;
        private RoundDto roundModel;

        public RoundManagerService(
            IOptions<RoundOptions> roundOptions,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RoundManagerService> logger)
        {
            this.roundOptions = roundOptions.Value;
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();

            try
            {
                //get in memory cache
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
                using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(this.roundOptions.RoundDurationInSeconds));
                await ManageRoundAsync(ct);

                while (await timer.WaitForNextTickAsync(ct))
                {
                    using IServiceScope scope = serviceScopeFactory.CreateScope();
                    var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();

                    roundModel = roundService.GetNextActiveRoundModel();
                    await ManageRoundAsync(ct);
                }
            }
            catch (Exception e)
            {
                this.logger.LogError("Unandled exception {error}", e);
                this.logger.LogError("Starting {service} again!", nameof(RoundManagerService));
                await StartAsync(ct);
            }
        }

        private Task ManageRoundAsync(CancellationToken ct)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();
            var ticketModuleIntegration = scope.ServiceProvider.GetRequiredService<ITicketModuleIntegration>();
            var roundHub = scope.ServiceProvider.GetRequiredService<IHubContext<RoundHub, IRoundHubClient>>();

            var roundId = roundModel.Id;

            this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} started");
            roundHub.Clients.All.RoundStarted(roundId);

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

            return Task.CompletedTask;
        }

        private void OnRaceStartEvent(int roundId)
        {
            try
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();
                var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();
                var roundHub = scope.ServiceProvider.GetRequiredService<IHubContext<RoundHub, IRoundHubClient>>();

                this.logger.LogInformation($" {DateTime.UtcNow} : Round {roundId} locked");

                //get from memory cache
                //chech if we have minumon of 7 in cache
                // if not get rest from database
                roundService.LockRound(roundId);

                roundHub.Clients.All.RaceStartWithBetLock(roundId);

                var outcome = roundService.GenerateRoundOutcome(roundId);

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
