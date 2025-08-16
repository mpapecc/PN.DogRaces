using Microsoft.EntityFrameworkCore;
using PlayNirvana.Bll.DataContext.Repositories.Abstraction;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Contracts;
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

        public void ProcessRoundBets(RoundsForProcess roundsForProcess)
        {
            foreach (var roundOutcome in roundsForProcess.RoundOutcomes)
            {

                //in production scenarion here we could get 100 or even 1000 or more records
                //it would be good to use async enumerator so that all records are not buffered into memory befor processing but rather processed as stream
                var roundBets = this.betsRepository.Query()
                    .Where(x => x.RoundId == roundOutcome.RoundId)
                    .Include(x => x.DogPositions)
                    .ToList();

                if (!roundBets.Any())
                {
                    break;
                }

                //process all bets
                foreach (var bet in roundBets)
                {
                    ProcessBet(bet, roundOutcome);
                }

                this.betsRepository.Commit();
            }

            //process all sucess tickets THIS CAN BE MOVED TO TICKET SERVICE ???
            this.ticketService.UpdateSuccessTicketsToWon();
            this.ticketService.UpdateSuccessTicketsToLost();
        }

        private void ProcessBet(Bet bet, RoundOutcome roundOutcome)
        {
            if (bet.BetType == BetType.Position)
            {
                bet.BetStatus = BetStatus.Won;

                foreach (var dogPosition in bet.DogPositions)
                {
                    if (roundOutcome.RaceDogResults.ElementAt(dogPosition.Position).RacingDogId != dogPosition.RacingDogId)
                    {
                        bet.BetStatus = BetStatus.Lost;
                        break;
                    }
                }
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
