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

        public TicketService(
            Validator<Ticket> betValidators,
            TicketRoundsValidator ticketRoundsValidator,
            IRepository<Ticket> ticketRepository)
        {
            this.ticketValidator = betValidators;
            this.ticketRoundsValidator = ticketRoundsValidator;
            this.ticketRepository = ticketRepository;
        }

        public void ValidateAndCreateTicket(CreatTicketModel creatTicketModel)
        {
            var ticket = creatTicketModel.ToTicket();

            var validationResults = this.ticketValidator.Validate(ticket);
            var isValid = !validationResults.Any();

            if (!isValid)
            {
                throw new TicketValidationException(validationResults.Select(x => new TicketValidationException(x.Message)));
            }

            //make reservation in wallet

            var ticketRoundsValidatorResult = this.ticketRoundsValidator.Validate(ticket);

            if (!ticketRoundsValidatorResult.IsSucess)
            {
                //cancle reservation in wallet

                throw new TicketValidationException(ticketRoundsValidatorResult.Message);
            }

            ticket.TicketStatus = TicketStatus.Success;
            this.ticketRepository.Insert(ticket);
            this.ticketRepository.Commit();
        }
    
        public void UpdateSuccessTicketsToWon()
        {
            this.ticketRepository.Query()
                    .Where(x => x.TicketStatus == TicketStatus.Success && x.Bets.All(x => x.BetStatus == BetStatus.Won))
                    .ExecuteUpdate(set => set.SetProperty(x => x.TicketStatus, TicketStatus.Won));
        }

        public void UpdateSuccessTicketsToLost()
        {
            this.ticketRepository.Query()
                    .Where(x => x.TicketStatus == TicketStatus.Success && x.Bets.Any(x => x.BetStatus == BetStatus.Lost))
                    .ExecuteUpdate(set => set.SetProperty(x => x.TicketStatus, TicketStatus.Lost));
        }
    }
}
