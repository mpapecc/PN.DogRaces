using Microsoft.EntityFrameworkCore;
using PlayNirvana.TicketModule.Application.Models;
using PlayNirvana.TicketModule.Application.Repositories;
using PlayNirvana.TicketModule.Application.Validators;
using PlayNirvana.TicketModule.Application.Validators.TicketValidators;
using PlayNirvana.TicketModule.Common.Enums;
using PlayNirvana.TicketModule.Common.Exceptions;
using PlayNirvana.TicketModule.Domain.Entites;

namespace PlayNirvana.TicketModule.Application.Services
{
    public class TicketService
    {
        private readonly Validator<Ticket> ticketValidator;
        private readonly TicketRoundsValidator ticketRoundsValidator;
        private readonly ITicketModuleRepository<Ticket> ticketRepository;
        //private readonly WalletService walletService;

        public TicketService(
            Validator<Ticket> betValidators,
            TicketRoundsValidator ticketRoundsValidator,
            ITicketModuleRepository<Ticket> ticketRepository)
        {
            ticketValidator = betValidators;
            this.ticketRoundsValidator = ticketRoundsValidator;
            this.ticketRepository = ticketRepository;
            //this.walletService = walletService;
        }

        public void ValidateAndCreateTicket(CreateTicketModel creatTicketModel)
        {
            var ticket = creatTicketModel.ToTicket();

            ValidateTicket(ticket);

            //make reservation in wallet

            //walletService.ReserveAmonunt(creatTicketModel.TicketId, creatTicketModel.BetAmount);

            var ticketRoundsValidatorResult = ticketRoundsValidator.Validate(ticket);

            if (!ticketRoundsValidatorResult.IsSucess)
            {
                //cancle reservation in wallet
                //walletService.RemoveReservation(creatTicketModel.TicketId);

                throw new TicketValidationException(ticketRoundsValidatorResult.Message);
            }

            ticket.TicketStatus = TicketStatus.Success;
            ticketRepository.Insert(ticket);
            ticketRepository.Commit();
        }

        //FOR THIS METHODS WE SHOULD CHECK IF BETS ARE SYSTEMATIC E.G. 2 OUT OF 3 FOR WINNING

        private void ValidateTicket(Ticket ticket)
        {

            var validationResults = ticketValidator.Validate(ticket);
            var isValid = !validationResults.Any();

            if (!isValid)
            {
                throw new TicketValidationException(validationResults.Select(x => new TicketValidationException(x.Message)));
            }
        }
        public void UpdateSuccessTicketsToWon(int roundId)
        {
            var wonTicketsInCurrentRoundQuery = ticketRepository.Query()
                    .Where(x => x.TicketStatus == TicketStatus.Success)
                    .Where(x => x.Bets.Any(x => x.RoundId == roundId) && x.Bets.All(x => x.BetStatus == BetStatus.Won));

            var aaa = wonTicketsInCurrentRoundQuery.ToQueryString();

            //handle wallet actions

            //this.walletService.ProcessReservation(12, TicketStatus.Success);

            wonTicketsInCurrentRoundQuery
                    .ExecuteUpdate(set => set.SetProperty(x => x.TicketStatus, TicketStatus.Won));
        }
        public void UpdateSuccessTicketsToLost(int roundId)
        {
            var lostTicketsInCurrentRoundQuery = ticketRepository.Query()
                .Where(x => x.TicketStatus == TicketStatus.Success)
                .Where(x => x.Bets.Any(x => x.RoundId == roundId) && x.Bets.Any(x => x.BetStatus == BetStatus.Lost));

            //handle wallet actions
            //this.walletService.ProcessReservation(12, TicketStatus.Lost);

            lostTicketsInCurrentRoundQuery
                    .ExecuteUpdate(set => set.SetProperty(x => x.TicketStatus, TicketStatus.Lost));
        }
    }
}
