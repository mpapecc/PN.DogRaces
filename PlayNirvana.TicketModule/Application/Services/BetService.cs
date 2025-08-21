using Microsoft.EntityFrameworkCore;
using PlayNirvana.CommonModule.Models;
using PlayNirvana.TicketModule.Application.Repositories;
using PlayNirvana.TicketModule.Common.Enums;
using PlayNirvana.TicketModule.Domain.Entites;

namespace PlayNirvana.TicketModule.Application.Services
{
    public class BetService
    {
        private readonly ITicketModuleRepository<Bet> betRepository;
        private readonly TicketService ticketService;

        public BetService(
            ITicketModuleRepository<Bet> betRepository,
            TicketService ticketService
            )
        {
            this.betRepository = betRepository;
            this.ticketService = ticketService;
        }

        public void ProcessRoundBets(RoundBetsProcessData roundBetsProcessData)
        {
            //in production scenarion here we could get 100 or even 1000 or more records
            //it would be good to use async enumerator so that all records are not buffered into memory befor processing but rather processed as stream
            var roundBets = betRepository.Query()
                .Where(x => x.RoundId == roundBetsProcessData.RoundId)
                .Include(x => x.DogPositions)
                .ToList();

            if (!roundBets.Any())
            {
                return;
            }

            //process all bets
            foreach (var bet in roundBets)
            {
                ProcessBet(bet, roundBetsProcessData.RaceDogsResult);
            }

            betRepository.Commit();

            //process all sucess tickets THIS CAN BE MOVED TO TICKET SERVICE ???
            ticketService.UpdateSuccessTicketsToWon(roundBetsProcessData.RoundId);
            ticketService.UpdateSuccessTicketsToLost(roundBetsProcessData.RoundId);
        }

        private void ProcessBet(Bet bet, IEnumerable<RaceDogResultModel> raceDogsResult)
        {
            if (bet.BetType == BetType.Position)
            {
                bet.BetStatus = BetStatus.Won;

                foreach (var dogPosition in bet.DogPositions)
                {
                    if (raceDogsResult.ElementAt(dogPosition.Position).RacingDogId != dogPosition.RacingDogId)
                    {
                        bet.BetStatus = BetStatus.Lost;
                        break;
                    }
                }
            }
        }
    }
}
