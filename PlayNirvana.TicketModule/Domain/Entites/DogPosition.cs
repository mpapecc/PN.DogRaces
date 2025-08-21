using PlayNirvana.CommonModule.DataContext.BaseEntities;

namespace PlayNirvana.TicketModule.Domain.Entites
{
    public class DogPosition : BaseEntity
    {
        public int RacingDogId { get; set; }
        public int BetId { get; set; }
        public int Position { get; set; }
    }
}
