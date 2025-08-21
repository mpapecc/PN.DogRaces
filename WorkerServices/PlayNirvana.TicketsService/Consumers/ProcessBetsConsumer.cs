using MassTransit;
using PlayNirvana.Bll.Services;
using PlayNirvana.Shared.Contracts;

namespace PlayNirvana.TicketsService.Consumers
{
    public class ProcessBetsConsumer : IConsumer<RaceStartWithBetLock>
    {
        private readonly BetService betService;
        private readonly ILogger<ProcessBetsConsumer> logger;

        public ProcessBetsConsumer(
            BetService betService, ILogger<ProcessBetsConsumer> logger)
        {
            this.betService = betService;
            this.logger = logger;
        }

        public Task Consume(ConsumeContext<RaceStartWithBetLock> context)
        {
            this.logger.LogInformation($"Processing bets for round {context.Message.RoundId} at {DateTime.UtcNow}");

            this.betService.ProcessRoundBets(context.Message);
            return Task.CompletedTask;
        }
    }
}
