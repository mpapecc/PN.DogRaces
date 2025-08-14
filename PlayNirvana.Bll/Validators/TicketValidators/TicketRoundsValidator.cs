using PlayNirvana.Bll.DataContext.Repositories.Implementation;
using PlayNirvana.Domain.Entites;

namespace PlayNirvana.Bll.Validators.TicketValidators
{
    public class TicketRoundsValidator : IValidator<Ticket>
    {
        private readonly RoundRepository roundRepository;

        public TicketRoundsValidator(RoundRepository roundRepository)
        {
            this.roundRepository = roundRepository;
        }

        public ValidationResult Validate(Ticket ticket)
        {
            var ticketRounds = ticket.Bets.Select(x => x.RoundId).ToList();

            var areAllRoundsActive = roundRepository.ActiveRoundQuery().Any(x => ticketRounds.Contains(x.Id));

            if (!areAllRoundsActive)
                return ValidationResult.Failed("Can not place bet on round that is not active for betting");
            return ValidationResult.Sucess();
        }
    }
}
