using PlayNirvana.CommonModule.DataContext.BaseEntities;
using PlayNirvana.TicketModule.Common.Enums;

namespace PlayNirvana.TicketModule.Domain.Entites
{
    public class Ticket : BaseChangeTrackingEntity
    {
        public IEnumerable<Bet> Bets { get; set; }
        public double BetAmount { get; set; }
        public double WinAmount { get; set; }
        public TicketStatus TicketStatus { get; set; } = TicketStatus.Pending;
    }
}
