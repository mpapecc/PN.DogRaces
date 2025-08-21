using MassTransit;
using PlayNirvana.Bll.Models;
using PlayNirvana.Bll.Services;
using PlayNirvana.Shared.Contracts;

namespace PlayNirvana.Scheduler.BackgroundServices
{
    public class RoundManagerService : BackgroundService
    {
        private readonly IBus publish;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RoundManagerService> logger;
        private RoundModel roundModel;


        public RoundManagerService(IBus publish,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RoundManagerService> logger)
        {
            this.publish = publish;
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();

            this.roundModel = roundService.GetNextActiveRoundModel();

            if (this.roundModel.Start >= DateTime.UtcNow)
            {
                roundService.TranslateActiveAndIdleRoundsStartInFuture();
                this.roundModel = roundService.GetNextActiveRoundModel();
            }

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            await Task.Delay(this.roundModel.Start - DateTime.UtcNow);
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
                this.logger.LogError(e.Message);
            }
        }

        private async Task ManageRoundAsync(bool isFirstExecution, CancellationToken ct)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();
            var scheduler = scope.ServiceProvider.GetRequiredService<IMessageScheduler>();
            var roundId = this.roundModel.Id;

            await publish.Publish(new RoundStarted(roundId), ct);

            var raceStartDelay =  Task.Delay(this.roundModel.CalculateUntilRaceStartWitBetLock());

            IEnumerable<RaceDogResultsRecord> roundOutcome = await raceStartDelay.ContinueWith((task) =>
            {
                roundService.LockRoundAsync(roundId);
                roundOutcome = roundService.GenerateRoundOutcome(roundId);
                publish.Publish(new RaceStartWithBetLock(roundId, roundOutcome), ct);
                return roundOutcome;
            }, ct);

            var roundFinishDelay = Task.Delay(this.roundModel.CalculateUntilRoundFinish());

            await roundFinishDelay.ContinueWith((task) =>
            {
                roundService.FinishRoundAsync(this.roundModel.Id);
                scheduler.SchedulePublish(
                roundModel.CalculateRoundFinishDate(),
                new RoundFinished(roundId, roundOutcome),
                ct);
            });

            this.roundModel = roundService.GetNextActiveRoundModel();
        }
    }
}
