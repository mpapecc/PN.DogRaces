using PlayNirvana.Shared.Enums;

namespace PlayNirvana.Bll.Models
{
    public class BetModel
    {
        public int RoundId { get; set; }
        public BetType BetType { get; set; }
        public BetStatus BetStatus { get; set; } = BetStatus.Pending;
        public IEnumerable<DogPositionModel> DogPositions { get; set; }
    }
}
