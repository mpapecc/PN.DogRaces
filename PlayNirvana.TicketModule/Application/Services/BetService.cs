using Microsoft.EntityFrameworkCore;
using PlayNirvana.CommonModule.Services;
using PlayNirvana.TicketModule.Application.Repositories;
using PlayNirvana.TicketModule.Common.Enums;
using PlayNirvana.TicketModule.Domain.Entites;

namespace PlayNirvana.TicketModule.Application.Services
{
    public class BetService
    {
        private readonly ITicketModuleRepository<Bet> betRepository;
        private readonly ITicketModuleRepository<DogPosition> dogPositionRepository;
        private readonly ITicketModuleRepository<RaceDogResult> raceDogResultRepository;
        private readonly TicketService ticketService;
        private readonly IExecuteUpdateOrDeleteBatcher executeUpdateOrDeleteBatcher;

        public BetService(
            ITicketModuleRepository<Bet> betRepository,
            ITicketModuleRepository<DogPosition> dogPositionRepository,
            ITicketModuleRepository<RaceDogResult> raceDogResultRepository,
            TicketService ticketService,
            IExecuteUpdateOrDeleteBatcher executeUpdateOrDeleteBatcher)
        {
            this.betRepository = betRepository;
            this.ticketService = ticketService;
            this.dogPositionRepository = dogPositionRepository;
            this.raceDogResultRepository = raceDogResultRepository;
            this.executeUpdateOrDeleteBatcher = executeUpdateOrDeleteBatcher;
        }

        public void ProcessRoundBets(int roundId)
        {
            var batchSize = 500;
            UpdatePositionBetTypeOnDbBatch(roundId, batchSize);
            UpdateRangeBetTypeOnDbBatch(roundId, batchSize);

            //process all sucess tickets THIS CAN BE MOVED TO TICKET SERVICE ???

            this.ticketService.UpdateSuccessTicketsToWon(roundId, batchSize);
            this.ticketService.UpdateSuccessTicketsToLost(roundId, batchSize);
        }

        private void UpdatePositionBetTypeOnDbBatch(int roundId, int batchsize)
        {
            int ProcessPositionBetTypeOnDb(IQueryable<Bet> query)
            {
                return query.ExecuteUpdate(x => x.SetProperty(x => x.BetStatus, b =>
                    dogPositionRepository.Query().Any(dp => dp.BetId == b.Id)
                    &&
                    !dogPositionRepository.Query()
                        .Where(dp => dp.BetId == b.Id)
                        .Any(dp =>
                            !raceDogResultRepository.Query().Any(r =>
                                r.RoundId == b.RoundId &&
                                r.RacingDogId == dp.RacingDogId &&
                                r.Place == dp.Position
                            )
                        )
                    ? BetStatus.Won
                    : BetStatus.Lost));
            }

            var query = this.betRepository.Query()
                .Where(b => b.RoundId == roundId && b.BetStatus == BetStatus.Pending && b.BetType == BetType.Position);

            this.executeUpdateOrDeleteBatcher.ExecuteUpdateOrDeleteInBatch(
                batchsize,
                query,
                ProcessPositionBetTypeOnDb
                );
        }

        private void UpdateRangeBetTypeOnDbBatch(int roundId, int batchsize)
        {
            int ProcessRangenBetTypeOnDb(IQueryable<Bet> query)
            {
                return query.ExecuteUpdate(x => x.SetProperty(x => x.BetStatus, b =>
                    dogPositionRepository.Query().Any(dp => dp.BetId == b.Id)
                    &&
                    dogPositionRepository.Query()
                        .Where(dp => dp.BetId == b.Id)
                        .Any(dp =>
                            raceDogResultRepository.Query().Any(r =>
                                r.RoundId == b.RoundId &&
                                r.RacingDogId == dp.RacingDogId &&
                                r.Place == dp.Position
                            )
                        )
                    ? BetStatus.Won
                    : BetStatus.Lost));
            }

            var query = this.betRepository.Query()
                .Where(b => b.RoundId == roundId && b.BetStatus == BetStatus.Pending && b.BetType == BetType.Range);

            this.executeUpdateOrDeleteBatcher.ExecuteUpdateOrDeleteInBatch(
                batchsize,
                query,
                ProcessRangenBetTypeOnDb
                );

        }
    }
}
