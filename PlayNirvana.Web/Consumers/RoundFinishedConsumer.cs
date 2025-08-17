using MassTransit;
using Microsoft.AspNetCore.SignalR;
using PlayNirvana.Bll.Repositories;
using PlayNirvana.Bll.Services;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Contracts;
using PlayNirvana.Web.GameHubs;

namespace PlayNirvana.Web.Consumers
{
    public class RoundFinishedConsumer : IConsumer<RoundFinished>
    {
        private readonly IHubContext<GameHub, IGameHubClient> gameHubClient;
        private readonly IRepository<RaceDogResult> raceDogRepository;
        private readonly RoundService roundService;
        private readonly ILogger<RoundFinishedConsumer> logger;

        public RoundFinishedConsumer(
            IHubContext<GameHub, IGameHubClient> gameHubClient,
            IRepository<RaceDogResult> raceDogRepository,
            RoundService roundService,
            ILogger<RoundFinishedConsumer> logger)
        {
            this.gameHubClient = gameHubClient;
            this.raceDogRepository = raceDogRepository;
            this.roundService = roundService;
            this.logger = logger;
        }

        public Task Consume(ConsumeContext<RoundFinished> context)
        {
            this.logger.LogInformation($"Consuming rounds finish event for round {context.Message.RoundId} => {DateTime.Now}");
            //finish race
            this.roundService.FinishRound(context.Message.RoundId);

            this.gameHubClient.Clients.All.RoundFinished(context.Message.RaceDogResults);

            return Task.CompletedTask;
        }
    }
}
