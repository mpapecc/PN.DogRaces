using MassTransit;
using PlayNirvana.Bll.DataContext.Repositories.Implementation;
using PlayNirvana.Bll.Services;
using PlayNirvana.Shared.Contracts;

namespace PlayNirvana.Scheduler.BackgroundServices
{
    public class RoundStarterService : SchedulableBackgroundService
    {
        private readonly IBus publish;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RoundStarterService> logger;
        private readonly int betLockBeforeStart = 2;
        private readonly int raceDuration = 10;

        public RoundStarterService(IBus publish,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RoundStarterService> logger)
        {
            this.publish = publish;
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        // every 10th minute of hour => actually every 10th minute - 5 sec for locking 
        // 55 9-59/10 * * * * 


        //this should not be scheduled job by cron expression but rather by Rounds.Start
        public override string CronExpression() => "*/20 * * * * *";

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //check if there are minumin 5 active rounds
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();

            roundService.GenerateRoundIfNeeded();
            return base.StartAsync(cancellationToken);
        }

        //WHAT IF THERE IS ERROR SOMEWHERE DOWN THE LINE??
        public override Task JobAsync(CancellationToken ct)
        {
            //WHY WE NEED CANCELLATIONTOKEN
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();
            var scheduler = scope.ServiceProvider.GetRequiredService<IMessageScheduler>();

            logger.LogInformation($"Lock round {betLockBeforeStart} seconds before start => {DateTime.Now}");

            var roundId = roundService.LockNextActiveRoundForBets();

            roundService.ActivateRound(roundId);

            //maybe this method call should be in ProcessRoundBets consumer, but since we need data also in 
            //RoundFinished consumer we are querying here and pass it to both consumers
            var roundsOutcome = roundService.GenerateRoundOutcome(roundId);

            Task.Delay(TimeSpan.FromSeconds(betLockBeforeStart), ct).Wait(); // IS THIS CORRECT ??

            //start race
            roundService.StartLockedRound(roundId);

            logger.LogInformation($"Publishing round for process with Id {roundId} {DateTime.Now}");
            var roundForProcess = new ProcessRoundBets(roundId, roundsOutcome);
            publish.Publish(roundForProcess, ct);

            logger.LogInformation($"Scheduling end of round with Id {roundId} {DateTime.Now}");
            var roundsFinished = new RoundFinished(roundId, roundsOutcome);
            return scheduler.SchedulePublish(DateTime.UtcNow + TimeSpan.FromSeconds(raceDuration), roundsFinished, ct);
        }
    }
}
