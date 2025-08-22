using Microsoft.EntityFrameworkCore;
using PlayNirvana.CommonModule.Services;
using PlayNirvana.TicketModule.Application.Models;
using PlayNirvana.TicketModule.Application.Repositories;
using PlayNirvana.TicketModule.Application.Validators;
using PlayNirvana.TicketModule.Application.Validators.TicketValidators;
using PlayNirvana.TicketModule.Common.Enums;
using PlayNirvana.TicketModule.Common.Exceptions;
using PlayNirvana.TicketModule.Domain.Entites;
using PlayNirvana.TicketModule.Integrations;

namespace PlayNirvana.TicketModule.Application.Services
{
    public class TicketService
    {
        private readonly Validator<Ticket> ticketValidator;
        private readonly TicketRoundsValidator ticketRoundsValidator;
        private readonly ITicketModuleRepository<Ticket> ticketRepository;
        private readonly IPaymentModuleIntegration paymentModuleIntegration;
        private readonly IExecuteUpdateOrDeleteBatcher executeUpdateOrDeleteBatcher;

        public TicketService(
            Validator<Ticket> betValidators,
            TicketRoundsValidator ticketRoundsValidator,
            ITicketModuleRepository<Ticket> ticketRepository,
            IPaymentModuleIntegration paymentModuleIntegration,
            IExecuteUpdateOrDeleteBatcher executeUpdateOrDeleteBatcher)
        {
            ticketValidator = betValidators;
            this.ticketRoundsValidator = ticketRoundsValidator;
            this.ticketRepository = ticketRepository;
            this.paymentModuleIntegration = paymentModuleIntegration;
            this.executeUpdateOrDeleteBatcher = executeUpdateOrDeleteBatcher;
        }

        public void ValidateAndCreateTicket(CreateTicketCommand creatTicketCommand)
        {
            var ticket = creatTicketCommand.ToTicket();

            ValidateTicket(ticket);

            var reservationId = this.paymentModuleIntegration.ReserveAmount(creatTicketCommand.BetAmount);

            var ticketRoundsValidatorResult = this.ticketRoundsValidator.Validate(ticket);

            if (!ticketRoundsValidatorResult.IsSucess)
            {
                this.paymentModuleIntegration.RemoveReservation(reservationId);

                throw new TicketValidationException(ticketRoundsValidatorResult.Message);
            }

            ticket.TicketStatus = TicketStatus.Success;
            ticket.PaymentReservationId = reservationId;

            this.ticketRepository.Insert(ticket);
            this.ticketRepository.Commit();
        }

        private void ValidateTicket(Ticket ticket)
        {

            var validationResults = ticketValidator.Validate(ticket);
            var isValid = !validationResults.Any();

            if (!isValid)
            {
                throw new TicketValidationException(validationResults.Select(x => new TicketValidationException(x.Message)));
            }
        }

        public void UpdateSuccessTicketsToWon(int roundId, int batchSize)
        {
            var wonTicketsInCurrentRoundQuery = ticketRepository.Query()
                    .Where(x => x.TicketStatus == TicketStatus.Success)
                    .Where(x => x.Bets.Any(x => x.RoundId == roundId) && x.Bets.All(x => x.BetStatus == BetStatus.Won));

            this.executeUpdateOrDeleteBatcher.ExecuteUpdateOrDeleteInBatch(
                batchSize,
                wonTicketsInCurrentRoundQuery,
                t => t.ExecuteUpdate(set => set.SetProperty(x => x.TicketStatus, TicketStatus.Won))
                );

            var wonTicketsPaymentReservationsIds = wonTicketsInCurrentRoundQuery.Select(x => x.PaymentReservationId).ToList();

            //since we have in-memory wallet for one player we are doing it all in-memory
            //for multi user scenarion with proper payment service we can also move this processing to db
            wonTicketsPaymentReservationsIds.ForEach(x => this.paymentModuleIntegration.ProcessReservation(x, isWinningTicket: true));
        }
        public void UpdateSuccessTicketsToLost(int roundId, int batchSize)
        {
            var lostTicketsInCurrentRoundQuery = ticketRepository.Query()
                .Where(x => x.TicketStatus == TicketStatus.Success)
                .Where(x => x.Bets.Any(x => x.RoundId == roundId) && x.Bets.Any(x => x.BetStatus == BetStatus.Lost));

            this.executeUpdateOrDeleteBatcher.ExecuteUpdateOrDeleteInBatch(
                batchSize,
                lostTicketsInCurrentRoundQuery,
                t => t.ExecuteUpdate(set => set.SetProperty(x => x.TicketStatus, TicketStatus.Lost))
                );

            var lostTicketsPaymentReservationsIds = lostTicketsInCurrentRoundQuery.Select(x => x.PaymentReservationId).ToList();

            //same comment as above
            lostTicketsPaymentReservationsIds.ForEach(x => this.paymentModuleIntegration.ProcessReservation(x, isWinningTicket: false));
        }
    }
}
