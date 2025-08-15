using MassTransit;
using PlayNirvana.Bll.Services;
using PlayNirvana.Shared.Contracts;

namespace PlayNirvana.BetsService.Consumers
{
    public class RoundsForProcessConsumer : IConsumer<RoundsForProcess>
    {
        private readonly BetService betService;

        public RoundsForProcessConsumer(BetService betService)
        {
            this.betService = betService;
        }

        public Task Consume(ConsumeContext<RoundsForProcess> context)
        {
            this.betService.ProcessRoundBets(context.Message.roundIds);
            return Task.CompletedTask;
        }
    }
}
