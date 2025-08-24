using PlayNirvana.CommonModule.DataContext.BaseEntities;
using PlayNirvana.TicketModule.Common.Enums;

namespace PlayNirvana.TicketModule.Domain.Entites
{
    public class Bet : BaseEntity
    {
        public int RoundId { get; set; }
        public BetType BetType { get; set; }
        public BetStatus BetStatus { get; set; }
        public IEnumerable<DogPosition> DogPositions { get; set; }
    }
}
