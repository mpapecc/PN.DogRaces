using PlayNirvana.CommonModule.DataContext.BaseEntities;

namespace PlayNirvana.CommonModule.SharedEntites
{
    public class RaceDogResult : BaseEntity
    {
        public int RacingDogId { get; set; }
        public int RoundId { get; set; }
        public int Place { get; set; }
    }
}
