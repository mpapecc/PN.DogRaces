using PlayNirvana.Common.DataContext.BaseEntities;

namespace PlayNirvana.RoundModule.Domain.Entites
{
    public class Bet : BaseChangeTrackingEntity
    {
        public int RoundId { get; set; }
        public IEnumerable<DogPosition> DogPositions { get; set; }
    }
}
