using MassTransit;
using Microsoft.AspNetCore.SignalR;
using PlayNirvana.Bll.DataContext.Repositories.Abstraction;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Contracts;
using PlayNirvana.Web.GameHubs;

namespace PlayNirvana.Web.Consumers
{
    public class RoundsFinishedConsumer : IConsumer<RoundsFinished>
    {
        private readonly IHubContext<GameHub, IGameHubClient> gameHubClient;
        private readonly IRepository<RaceDogResult> raceDogRepository;

        public RoundsFinishedConsumer(
            IHubContext<GameHub, IGameHubClient> gameHubClient,
            IRepository<RaceDogResult> raceDogRepository)
        {
            this.gameHubClient = gameHubClient;
            this.raceDogRepository = raceDogRepository;
        }

        public Task Consume(ConsumeContext<RoundsFinished> context)
        {
            var result = this.raceDogRepository.Query()
                .Where(x => context.Message.roundIds.Contains(x.RoundId))
                .GroupBy(x => x.RoundId)
                .ToList();
            
            this.gameHubClient.Clients.All.SendRoundResult(result);

            return Task.CompletedTask;
        }
    }
}
