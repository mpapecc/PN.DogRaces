using PlayNirvana.TicketModule.Common.Enums;

namespace PlayNirvana.TicketModule.Application.Models
{
    public class TicketModel
    {
        public double BetAmount { get; set; }
        public double WinAmount { get; set; }
        public TicketStatus TicketStatus { get; set; } = TicketStatus.Pending;
        public DateTime CreatedOn { get; set; }
    }
}
