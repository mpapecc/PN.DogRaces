using PlayNirvana.Common.Interfaces;
using PlayNirvana.TicketModule.Application.Validators;
using PlayNirvana.TicketModule.Domain.Entites;

namespace PlayNirvana.TicketModule.Application.Validators.TicketValidators
{
    public class TicketRoundsValidator : IValidator<Ticket>
    {
        private readonly IRoundModuleExternal roundExternal;

        public TicketRoundsValidator(IRoundModuleExternal roundExternal)
        {
            this.roundExternal = roundExternal;
        }

        public ValidationResult Validate(Ticket ticket)
        {
            var ticketRounds = ticket.Bets.Select(x => x.RoundId).ToList();

            var allRoundsActive = roundExternal.ActiveRoundIds();
            //this also handles case when round is locked and it is not present in active rounds list
            var areAllRoundsActive = !ticketRounds.Any(x => !allRoundsActive.Contains(x));

            if (!areAllRoundsActive)
                return ValidationResult.Failed("Can not place bet on round that is not active for betting");
            return ValidationResult.Sucess();
        }
    }
}
