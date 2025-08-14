using PlayNirvana.Bll.DataContext.Repositories.Implementation;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Enums;
using PlayNirvana.Scheduler.BackgroundServices.Abstraction;
using PlayNirvana.Bll.Services;

namespace PlayNirvana.Scheduler.BackgroundServices.Implementation
{
    public class RoundsGenerator : BackgroundServiceBase
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly int newRoundsThreshold = 10;
        private readonly int roundDuration = 10;
        public RoundsGenerator(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        //every day at midnight
        public override string CronExpression() => "*/30 * * * * *";

        public override Task JobAsync(CancellationToken ct)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var roundRepository = scope.ServiceProvider.GetRequiredService<RoundRepository>();
            var roundService = scope.ServiceProvider.GetRequiredService<RoundService>();

            // betting time (7) + race (3) = 10 min
            // that means in a one day there can be 144 races
            // we are actually generating 216 races (days and half worth) so that we dont have
            // issues in case of latency in midnight
            // we will also check if ther are more then 200 iddle races in if so we will skipp generation

            var idleRoundsCount = roundRepository.GetIdleRoundsCount();
            IEnumerable<Round> rounds;

            if (idleRoundsCount >= this.newRoundsThreshold)
            {
                return Task.CompletedTask;
            }

            // should add BeforeJob method to create rounds when Worker starts so that user
            // doesnt have to wait for next execution for placing bet
            if (idleRoundsCount == 0)
            {
                var nextRoundStartTime = roundService.CalculateNextRoundStart();

                rounds = roundService.GenerateRounds(nextRoundStartTime, rounds => roundService.ActivateFirstNRounds(rounds, 5));
            }
            else
            {
                var lastRoundStartTime = roundRepository.GetLastIdleRoundStartDate();
                rounds = roundService.GenerateRounds(lastRoundStartTime);
            }

            roundRepository.InsertRange(rounds);
            roundRepository.Commit();

            return Task.CompletedTask;
        }
    }
}
