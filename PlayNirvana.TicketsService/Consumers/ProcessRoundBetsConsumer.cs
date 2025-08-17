using MassTransit;
using PlayNirvana.Bll.Services;
using PlayNirvana.Shared.Contracts;

namespace PlayNirvana.BetsService.Consumers
{
    public class ProcessRoundBetsConsumer : IConsumer<ProcessRoundBets>
    {
        private readonly BetService betService;
        private readonly ILogger<ProcessRoundBetsConsumer> logger;

        public ProcessRoundBetsConsumer(BetService betService, ILogger<ProcessRoundBetsConsumer> logger)
        {
            this.betService = betService;
            this.logger = logger;
        }

        public Task Consume(ConsumeContext<ProcessRoundBets> context)
        {
            this.logger.LogInformation($"consuming procees bets for round {context.Message.RoundId} at {DateTime.Now}");
            this.betService.ProcessRoundBets(context.Message);
            return Task.CompletedTask;
        }
    }
}
