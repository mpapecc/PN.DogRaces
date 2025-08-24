using PlayNirvana.CommonModule;
using PlayNirvana.CommonModule.Services;
using PlayNirvana.TicketModule.Domain.Entites;
using PlayNirvana.TicketModule.Integrations;

namespace PlayNirvana.TicketModule.Application.Validators
{
    public class TicketWinAmountValidator : IValidator<Ticket>
    {
        public static readonly string message = "Bet win amount can not be greater then 100 00";
        public static readonly int maxWinAmount = 100_000;

        public ValidationResult Validate(Ticket ticket)
        {
            if (ticket.WinAmount > maxWinAmount)
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

    public class TicketAmountValidator : IValidator<Ticket>
    {
        public static readonly string message = "Bet amaount can not be greater then 10 000";
        public static readonly int maxBetAmount = 10_000;
        
        public ValidationResult Validate(Ticket ticket)
        {
            if (ticket.BetAmount > maxBetAmount)
                return ValidationResult.Failed(message);
            return ValidationResult.Sucess();
        }
    }
}
