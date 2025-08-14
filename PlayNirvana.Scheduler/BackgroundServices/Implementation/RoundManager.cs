using System.Text;
using System.Text.Json;
using PlayNirvana.Bll.DataContext.Repositories.Implementation;
using PlayNirvana.Bll.Services;
using PlayNirvana.Scheduler.BackgroundServices.Abstraction;
using RabbitMQ.Client;

namespace PlayNirvana.Scheduler.BackgroundServices.Implementation
{
    public class RoundManager : BackgroundServiceBase
    {
        private readonly ILogger<RoundManager> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly int betLockBeforeStart = 2;
        private readonly int raceDuration = 5;

        public RoundManager(ILogger<RoundManager> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        // every 10th minute of hour => actually every 10th minute - 5 sec for locking 
        // 55 9-59/10 * * * * 

        public override string CronExpression() => "*/8 * * * * *";
        
        // Handle service shuting down...all races must be finished
        public override async Task JobAsync(CancellationToken ct)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();
            var channel = scope.ServiceProvider.GetRequiredService<IChannel>();

            //lock race for 2 seconds to calculate winner
            var roundIds = roundService.LockNextActiveRoundForBets();

            //open bets for next idle run
            roundService.ActivateNextNRounds();

            //calculate round outcome adn save to DB
            roundService.GenerateRoundOutcome(roundIds);

            //Notify beting service outcome is generated and process all bets
            await channel.BasicPublishAsync(
                          exchange: string.Empty,
                          routingKey: "bets-processor",
                          mandatory: true,
                          basicProperties: new BasicProperties { Persistent = true },
                          body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(roundIds))
                          );

            await Task.Delay(TimeSpan.FromSeconds(this.betLockBeforeStart));

            //start race
            roundService.StartLockedRound();

            await Task.Delay(TimeSpan.FromSeconds(this.raceDuration));

            //finish race
            roundService.FinishInProgressRound();
        }
    }
}
