//using MassTransit;
//using Microsoft.AspNetCore.SignalR;
//using PlayNirvana.Bll.Repositories;
//using PlayNirvana.Bll.Services;
//using PlayNirvana.Common.DataContext;
//using PlayNirvana.Domain.Entites;

//using PlayNirvana.Shared.Contracts;
//using PlayNirvana.Web.GameHubs;

//namespace PlayNirvana.Web.Consumers
//{
//    public class RoundFinishedConsumer : IConsumer<RoundFinished>
//    {
//        private readonly IHubContext<GameHub, IGameHubClient> gameHubClient;
//        private readonly RoundService roundService;
//        private readonly ILogger<RoundFinishedConsumer> logger;

//        public RoundFinishedConsumer(
//            IHubContext<GameHub, IGameHubClient> gameHubClient,
//            IRepository<RaceDogResult> raceDogRepository,
//            RoundService roundService,
//            ILogger<RoundFinishedConsumer> logger)
//        {
//            this.gameHubClient = gameHubClient;
//            this.roundService = roundService;
//            this.logger = logger;
//        }

//        public Task Consume(ConsumeContext<RoundFinished> context)
//        {
//            this.logger.LogInformation($"Notifiying clients round {context.Message.RoundId} is finished => {DateTime.UtcNow}");

//            this.gameHubClient.Clients.All.RoundFinished(new { context.Message.RoundId, Order = this.roundService.GetRoundOutcome(context.Message.RoundId) });

//            return Task.CompletedTask;
//        }
//    }
//}
