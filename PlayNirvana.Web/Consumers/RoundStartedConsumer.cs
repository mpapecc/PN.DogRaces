//using MassTransit;
//using Microsoft.AspNetCore.SignalR;
//using PlayNirvana.Bll.Repositories;
//using PlayNirvana.Bll.Services;
//using PlayNirvana.Common.DataContext;
//using PlayNirvana.Domain.Entites;
//using PlayNirvana.Round.Repositories;
//using PlayNirvana.RoundModule;
//using PlayNirvana.RoundModule.Entites;
//using PlayNirvana.Shared.Contracts;
//using PlayNirvana.Web.GameHubs;

//namespace PlayNirvana.Web.Consumers
//{
//    public class RoundStartedConsumer : IConsumer<RoundStarted>
//    {
//        private readonly IHubContext<GameHub, IGameHubClient> gameHubClient;
//        private readonly IRepository<RaceDogResult> raceDogRepository;
//        private readonly RoundService roundService;
//        private readonly ILogger<RoundFinishedConsumer> logger;

//        public RoundStartedConsumer(
//            IHubContext<GameHub, IGameHubClient> gameHubClient,
//            IRepository<RaceDogResult> raceDogRepository,
//            RoundService roundService,
//            ILogger<RoundFinishedConsumer> logger)
//        {
//            this.gameHubClient = gameHubClient;
//            this.raceDogRepository = raceDogRepository;
//            this.roundService = roundService;
//            this.logger = logger;
//        }

//        public Task Consume(ConsumeContext<RoundStarted> context)
//        {
//            this.logger.LogInformation($"Notifying clients round {context.Message.RoundId} is started=> {DateTime.UtcNow}");

//            this.gameHubClient.Clients.All.RoundStarted(context.Message.RoundId);

//            return Task.CompletedTask;
//        }
//    }
//}
