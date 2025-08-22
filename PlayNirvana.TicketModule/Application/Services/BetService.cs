using Microsoft.EntityFrameworkCore;
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

        public BetService(
            ITicketModuleRepository<Bet> betRepository,
            ITicketModuleRepository<DogPosition> dogPositionRepository,
            ITicketModuleRepository<RaceDogResult> raceDogResultRepository,
            TicketService ticketService)
        {
            this.betRepository = betRepository;
            this.ticketService = ticketService;
            this.dogPositionRepository = dogPositionRepository;
            this.raceDogResultRepository = raceDogResultRepository;
        }

        public void ProcessRoundBets(int roundId)
        {
            var positionBetsUpdate = ProcessPositionBetTypeOnDbAsync(roundId);
            var rangeBetsUpdate = ProcessRangeBetTypeOnDbAsync(roundId);

            Task.WaitAll(positionBetsUpdate, rangeBetsUpdate);

            //process all sucess tickets THIS CAN BE MOVED TO TICKET SERVICE ???

            this.ticketService.UpdateSuccessTicketsToWon(roundId);
            //this.ticketService.UpdateSuccessTicketsToLost(roundBetsProcessData.RoundId);
        }

        //private void ProcessBet(Bet bet, IEnumerable<RaceDogResultModel> raceDogsResult)
        //{
        //    if (bet.BetType == BetType.Position)
        //    {
        //        ProcessPositionBetType(bet, raceDogsResult);
        //    }
        //}

        //private void ProcessPositionBetType(Bet bet, IEnumerable<RaceDogResultModel> raceDogsResult)
        //{
        //    bool isBetWining(Bet bet, IEnumerable<RaceDogResultModel> raceDogsResult)
        //    {
        //        return bet.DogPositions.All(x => raceDogsResult.ElementAt(x.Position).RacingDogId == x.RacingDogId);
        //    }

        //    bet.BetStatus = isBetWining(bet, raceDogsResult) ? BetStatus.Won : BetStatus.Lost;
        //}
        
        private Task ProcessPositionBetTypeOnDbAsync(int roundId)
        {
            return this.betRepository.Query()
                .Where(b => b.RoundId == roundId && b.BetType == BetType.Position)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(
                        b => b.BetStatus,
                        b =>
                            (
                                dogPositionRepository.Query().Any(dp => dp.BetId == b.Id)
                                &&
                                // and there does NOT exist a DogPosition with no matching RaceDogResult
                                !dogPositionRepository.Query()
                                    .Where(dp => dp.BetId == b.Id)
                                    .Any(dp =>
                                        !raceDogResultRepository.Query().Any(r =>
                                            r.RoundId == b.RoundId &&
                                            r.RacingDogId == dp.RacingDogId &&
                                            r.Place == dp.Position
                                        )
                                    )
                            )
                            ? BetStatus.Won
                            : BetStatus.Lost
                    )
                );
        }

        private Task ProcessRangeBetTypeOnDbAsync(int roundId)
        {
            return this.betRepository.Query()
            .Where(b => b.RoundId == roundId && b.BetType == BetType.Range)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    b => b.BetStatus,
                    b =>
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
                        : BetStatus.Lost
                )
    );
        }
    }
}
