using MassTransit;
using Microsoft.AspNetCore.SignalR;
using PlayNirvana.Bll.DataContext.Repositories.Abstraction;
using PlayNirvana.Bll.Services;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Contracts;
using PlayNirvana.Web.GameHubs;

namespace PlayNirvana.Web.Consumers
{
    public class RoundStartedConsumer : IConsumer<RoundStarted>
    {
        private readonly IHubContext<GameHub, IGameHubClient> gameHubClient;
        private readonly IRepository<RaceDogResult> raceDogRepository;
        private readonly RoundService roundService;
        private readonly ILogger<RoundFinishedConsumer> logger;

        public RoundStartedConsumer(
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

        public Task Consume(ConsumeContext<RoundStarted> context)
        {
            this.logger.LogInformation($"Consuming rounds started event for round {context.Message.RoundId} => {DateTime.Now}");

            this.gameHubClient.Clients.All.RoundStarted(context.Message.RoundId);

            return Task.CompletedTask;
        }
    }
}
