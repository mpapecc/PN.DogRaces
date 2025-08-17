using PlayNirvana.Domain.Entites.BaseEntities;
using PlayNirvana.Shared.Enums;

namespace PlayNirvana.Domain.Entites
{
    public class Round : BaseChangeTrackingEntity
    {
        public DateTime Start { get; set; }
        public RoundStatus RoundStatus { get; set; }
    }
}
