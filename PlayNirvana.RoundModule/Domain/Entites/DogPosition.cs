using PlayNirvana.Common.DataContext.BaseEntities;

namespace PlayNirvana.RoundModule.Domain.Entites
{
    public class DogPosition : BaseEntity
    {
        public int RacingDogId { get; set; }
        public int BetId { get; set; }
        public int Position { get; set; }
    }
}
