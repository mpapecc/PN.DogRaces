using Microsoft.Extensions.Options;
using PlayNirvana.CommonModule.Services;
using PlayNirvana.TicketModule.Common.Options;
using PlayNirvana.TicketModule.Domain.Entites;
using PlayNirvana.TicketModule.Integrations;

namespace PlayNirvana.TicketModule.Application.Validators
{
    public class TicketWinAmountValidator : IValidator<Ticket>
    {
        public static readonly string message = "Bet win amount can not be greater then 100 00";
        private readonly TicketOptions ticketOptions;

        public TicketWinAmountValidator(IOptions<TicketOptions> ticketOptions)
        {
            this.ticketOptions = ticketOptions.Value;
        }

        public ValidationResult Validate(Ticket ticket)
        {
            if (ticket.WinAmount > ticketOptions.MaxWinAmount)
                return ValidationResult.Failed(message);
            return ValidationResult.Sucess();
        }
    }

    public class TicketRoundsValidator : IValidator<Ticket>
    {
        public static readonly string message = "Can not place bet on round that is not active for betting";

        private readonly IRoundModuleIntegration roundModuleIntegration;

        public TicketRoundsValidator(IRoundModuleIntegration roundModuleIntegration)
        {
            this.roundModuleIntegration = roundModuleIntegration;
        }

        public ValidationResult Validate(Ticket ticket)
        {
            var ticketRounds = ticket.Bets.Select(x => x.RoundId).ToList();

            var allRoundsActive = roundModuleIntegration.ActiveRoundIds();
            //this also handles case when round is locked and it is not present in active rounds list
            var areAllRoundsActive = !ticketRounds.Any(x => !allRoundsActive.Contains(x));

            if (!areAllRoundsActive)
                return ValidationResult.Failed(message);
            return ValidationResult.Sucess();
        }
    }

    public class TicketBetAmountValidator : IValidator<Ticket>
    {
        public static readonly string message = "Bet amaount can not be greater then 10 000";
        private readonly TicketOptions ticketOptions;

        public TicketBetAmountValidator(IOptions<TicketOptions> ticketOptions)
        {
            this.ticketOptions = ticketOptions.Value;
        }

        public ValidationResult Validate(Ticket ticket)
        {
            if (ticket.BetAmount > ticketOptions.MaxBetAmount)
                return ValidationResult.Failed(message);
            return ValidationResult.Sucess();
        }
    }
}
