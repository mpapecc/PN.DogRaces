using Microsoft.EntityFrameworkCore;
using PlayNirvana.Bll.DataContext.Repositories.Abstraction;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Enums;

namespace PlayNirvana.Bll.Services
{
    public class BetService
    {
        private readonly IRepository<Bet> betsRepository;
        private readonly IRepository<RaceDogResult> raceDogResultRepository;
        private readonly TicketService ticketService;

        public BetService(
            IRepository<Bet> betsRepository, 
            IRepository<RaceDogResult> raceDogResultRepository,
            TicketService ticketService
            )
        {
            this.betsRepository = betsRepository;
            this.raceDogResultRepository = raceDogResultRepository;
            this.ticketService = ticketService;
        }

        public void ProcessRoundBets(IEnumerable<int> roundIds)
        {
            foreach (var id in roundIds)
            {
                var orderdDogsForRound = this.raceDogResultRepository.Query()
                                                    .Where(x => x.RoundId == id)
                                                    .OrderBy(x => x.Place)
                                                    .ToList();

                PrintDogsOrder(orderdDogsForRound);

                var roundBets = this.betsRepository.Query()
                    .Where(x => x.RoundId == id)
                    .Include(x => x.DogPositions)
                    .ToList();

                //process all bets
                foreach (var bet in roundBets)
                {
                    if(bet.BetType == BetType.Position)
                    {
                        bet.BetStatus = BetStatus.Won;

                        foreach (var dogPosition in bet.DogPositions)
                        {
                            if (orderdDogsForRound[dogPosition.Position].RacingDogId != dogPosition.RacingDogId)
                            {
                                bet.BetStatus = BetStatus.Lost;
                                break;
                            }
                        }
                    }
                }

                //process all pending tickets THIS CAN BE MOVED TO TICKET SERVICE ???

                //update pending tickets where all bets have won
                this.ticketService.UpdateSuccessTicketsToWon();

                //update pending tickets where any bet has lost
                this.ticketService.UpdateSuccessTicketsToLost();

            }
        }

        private void PrintDogsOrder(IEnumerable<RaceDogResult> raceDogResults)
        {
            foreach (var result in raceDogResults)
            {
                Console.WriteLine($"Place {result.Place} => {result.RacingDogId}");
            }
        }
    }
}
