using Microsoft.EntityFrameworkCore;
using PlayNirvana.Bll.DataContext.Repositories.Abstraction;
using PlayNirvana.Bll.Models.TicketModels;
using PlayNirvana.Bll.Validators;
using PlayNirvana.Bll.Validators.TicketValidators;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Enums;
using PlayNirvana.Shared.Exceptions;

namespace PlayNirvana.Bll.Services
{
    public class TicketService
    {
        private readonly Validator<Ticket> ticketValidator;
        private readonly TicketRoundsValidator ticketRoundsValidator;
        private readonly IRepository<Ticket> ticketRepository;
        private readonly WalletService walletService;

        public TicketService(
            Validator<Ticket> betValidators,
            TicketRoundsValidator ticketRoundsValidator,
            IRepository<Ticket> ticketRepository,
            WalletService walletService)
        {
            this.ticketValidator = betValidators;
            this.ticketRoundsValidator = ticketRoundsValidator;
            this.ticketRepository = ticketRepository;
            this.walletService = walletService;
        }

        public void ValidateAndCreateTicket(CreateTicketModel creatTicketModel)
        {
            var ticket = creatTicketModel.ToTicket();

            var validationResults = this.ticketValidator.Validate(ticket);
            var isValid = !validationResults.Any();

            if (!isValid)
            {
                throw new TicketValidationException(validationResults.Select(x => new TicketValidationException(x.Message)));
            }

            //make reservation in wallet

            this.walletService.ReserveAmonunt(creatTicketModel.TicketId, creatTicketModel.BetAmount);

            var ticketRoundsValidatorResult = this.ticketRoundsValidator.Validate(ticket);

            if (!ticketRoundsValidatorResult.IsSucess)
            {
                //cancle reservation in wallet
                this.walletService.RemoveReservation(creatTicketModel.TicketId);

                throw new TicketValidationException(ticketRoundsValidatorResult.Message);
            }

            ticket.TicketStatus = TicketStatus.Success;
            this.ticketRepository.Insert(ticket);
            this.ticketRepository.Commit();
        }

        //FOR THIS METHODS WE SHOULD CHECK IF BETS ARE SYSTEMATIC E.G. 2 OUT OF 3 FOR WINNING

        public void UpdateSuccessTicketsToWon()
        {
            var wonTicketsQuery = this.ticketRepository.Query()
                    .Where(x => x.TicketStatus == TicketStatus.Success && x.Bets.All(x => x.BetStatus == BetStatus.Won));

            //handle wallet actions

            this.walletService.ProcessReservation(12, TicketStatus.Success);

            wonTicketsQuery
                    .ExecuteUpdate(set => set.SetProperty(x => x.TicketStatus, TicketStatus.Won));
        }

        public void UpdateSuccessTicketsToLost()
        {
            var lostTicketsQuery = this.ticketRepository.Query()
                    .Where(x => x.TicketStatus == TicketStatus.Success && x.Bets.Any(x => x.BetStatus == BetStatus.Lost));

            //handle wallet actions
            //this.walletService.ProcessReservation(12, TicketStatus.Lost);

            lostTicketsQuery
                    .ExecuteUpdate(set => set.SetProperty(x => x.TicketStatus, TicketStatus.Lost));
        }

        public IEnumerable<TicketModel> GetSuccessTickets()
        {
            return this.ticketRepository.Query()
                .Where(x => x.TicketStatus == TicketStatus.Success)
                .Select(x => new TicketModel()
                {
                    BetAmount = x.BetAmount,
                    CreatedOn = x.CreatedOn,
                    TicketStatus = x.TicketStatus,
                    WinAmount = x.WinAmount
                });
        }

        public TicketDetailsModel GetTicketDetails(int ticketId)
        {
            var ticketDetails = this.ticketRepository.Query()
                .Where(x => x.Id == ticketId)
                .Select(x => new TicketDetailsModel()
                {
                    BetAmount = x.BetAmount,
                    CreatedOn = x.CreatedOn,
                    TicketStatus = x.TicketStatus,
                    WinAmount = x.WinAmount
                })
                .FirstOrDefault();
            
            if(ticketDetails == null)
            {
                throw new ArgumentException($"Ticket with Id {ticketId} does not exists.");
            }

            return ticketDetails;
        }
    }
}
