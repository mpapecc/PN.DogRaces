using PlayNirvana.CommonModule.DataContext.BaseEntities;

namespace PlayNirvana.TicketModule.Domain.Entites
{
    public class RaceDogResult : BaseEntity
    {
        public int RacingDogId { get; set; }
        public int RoundId { get; set; }
        public int Place { get; set; }
    }
}
