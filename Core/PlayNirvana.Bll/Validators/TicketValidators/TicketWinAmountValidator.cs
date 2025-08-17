using System.Net.Sockets;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Exceptions;

namespace PlayNirvana.Bll.Validators.TicketValidators
{
    public class TicketWinAmountValidator : IValidator<Ticket>
    {
        public ValidationResult Validate(Ticket ticket)
        {
            if (ticket.WinAmount > 100000)
                return ValidationResult.Failed("Bet win amount can not be greater then 100 000");
            return ValidationResult.Sucess();
        }
    }
}
