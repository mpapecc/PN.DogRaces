using MassTransit;
using Microsoft.AspNetCore.SignalR;
using PlayNirvana.Bll.DataContext.Repositories.Abstraction;
using PlayNirvana.Bll.Services;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Contracts;
using PlayNirvana.Web.GameHubs;

namespace PlayNirvana.Web.Consumers
{
    public class RoundsFinishedConsumer : IConsumer<RoundsFinished>
    {
        private readonly IHubContext<GameHub, IGameHubClient> gameHubClient;
        private readonly IRepository<RaceDogResult> raceDogRepository;
        private readonly RoundService roundService;
        private readonly ILogger<RoundsFinishedConsumer> logger;

        public RoundsFinishedConsumer(
            IHubContext<GameHub, IGameHubClient> gameHubClient,
            IRepository<RaceDogResult> raceDogRepository,
            RoundService roundService,
            ILogger<RoundsFinishedConsumer> logger)
        {
            this.gameHubClient = gameHubClient;
            this.raceDogRepository = raceDogRepository;
            this.roundService = roundService;
            this.logger = logger;
        }

        public Task Consume(ConsumeContext<RoundsFinished> context)
        {
            this.logger.LogInformation($"Consuming rounds finish event => {DateTime.Now}");
            //finish race
            this.roundService.FinishInProgressRound();

            var result = this.raceDogRepository.Query()
                .Where(x => context.Message.roundIds.Contains(x.RoundId))
                .GroupBy(x => x.RoundId)
                .ToList();
            
            this.gameHubClient.Clients.All.SendRoundResult(result);

            return Task.CompletedTask;
        }
    }
}
