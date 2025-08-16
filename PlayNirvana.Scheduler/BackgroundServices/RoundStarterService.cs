using MassTransit;
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
        private readonly int raceDuration = 5;

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

        public override string CronExpression() => "*/8 * * * * *";

        public override Task JobAsync(CancellationToken ct)
        {
            //WHY WE NEED CANCELLATIONTOKEN
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();
            var scheduler = scope.ServiceProvider.GetRequiredService<IMessageScheduler>();

            var roundIds = roundService.LockNextActiveRoundForBets(); // IS IT NEEDED ???

            logger.LogInformation($"Lock rounds {betLockBeforeStart} seconds before start => {DateTime.Now}");

            roundService.ActivateNextNRounds();

            var roundsOutcome = roundService.GenerateRoundOutcome(roundIds);

            Task.Delay(TimeSpan.FromSeconds(betLockBeforeStart), ct).Wait(); // IS THIS CORRECT ??

            //start race
            roundService.StartLockedRound();

            logger.LogInformation($"Publishing rounds for process {DateTime.Now}");
            var roundForProcess = new RoundsForProcess(roundsOutcome);
            publish.Publish(roundForProcess, ct);

            logger.LogInformation($"Scheduling end of rounds {DateTime.Now}");
            var roundsFinished = new RoundsFinished(roundIds);
            return scheduler.SchedulePublish(DateTime.UtcNow + TimeSpan.FromSeconds(raceDuration), roundsFinished, ct);
        }
    }
}
