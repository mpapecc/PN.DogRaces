using PlayNirvana.Domain.Entites.BaseEntities;
using PlayNirvana.Shared.Enums;

namespace PlayNirvana.Domain.Entites
{
    public class Bet : BaseChangeTrackingEntity
    {
        public Round Round { get; set; }
        public int RoundId { get; set; }
        public BetType BetType { get; set; }
        public BetStatus BetStatus { get; set; } = BetStatus.Pending;
        public IEnumerable<DogPosition> DogPositions { get; set; }
    }
}
