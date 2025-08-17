using PlayNirvana.Domain.Entites.BaseEntities;

namespace PlayNirvana.Domain.Entites
{
    public class RaceDogResult : BaseEntity
    {
        public RacingDog RacingDog { get; set; }
        public int RacingDogId { get; set; }
        public Round Round { get; set; }
        public int RoundId { get; set; }
        public int Place { get; set; }
    }
}
