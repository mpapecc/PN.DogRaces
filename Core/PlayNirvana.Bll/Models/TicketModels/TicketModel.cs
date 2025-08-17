using PlayNirvana.Shared.Enums;

namespace PlayNirvana.Bll.Models.TicketModels
{
    public class TicketModel
    {
        public double BetAmount { get; set; }
        public double WinAmount { get; set; }
        public TicketStatus TicketStatus { get; set; } = TicketStatus.Pending;
        public DateTime CreatedOn { get; set; }
    }
}
