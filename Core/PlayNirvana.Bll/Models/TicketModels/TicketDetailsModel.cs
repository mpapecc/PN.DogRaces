namespace PlayNirvana.Bll.Models.TicketModels
{
    public class TicketDetailsModel : TicketModel
    {
        public IEnumerable<BetModel> Bets { get; set; }
    }
}
