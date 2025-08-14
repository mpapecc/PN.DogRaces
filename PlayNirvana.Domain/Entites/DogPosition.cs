using PlayNirvana.Domain.Entites.BaseEntities;

namespace PlayNirvana.Domain.Entites
{
    public class DogPosition : BaseEntity
    {
        public RacingDog RacingDog { get; set; }
        public int RacingDogId { get; set; }
        public Bet Bet { get; set; }
        public int BetId { get; set; }
        public int Position { get; set; }
    }
}
