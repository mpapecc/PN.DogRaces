using PlayNirvana.TicketModule.Application.Validators;
using PlayNirvana.TicketModule.Domain.Entites;

namespace PlayNirvana.TicketModule.Application.Validators.TicketValidators
{
    public class TicketAmountValidator : IValidator<Ticket>
    {
        public ValidationResult Validate(Ticket ticket)
        {
            if (ticket.BetAmount > 10000)
                return ValidationResult.Failed("Bet amaount can not be greater then 10 000");
            return ValidationResult.Sucess();
        }
    }
}
