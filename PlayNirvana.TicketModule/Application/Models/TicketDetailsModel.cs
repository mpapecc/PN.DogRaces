namespace PlayNirvana.TicketModule.Application.Models
{
    public class TicketDetailsModel : TicketModel
    {
        public IEnumerable<BetModel> Bets { get; set; }
    }
}
