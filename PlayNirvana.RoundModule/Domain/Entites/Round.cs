using PlayNirvana.CommonModule.DataContext.BaseEntities;
using PlayNirvana.RoundModule.Common.Enums;

namespace PlayNirvana.RoundModule.Domain.Entites
{
    public class Round : BaseChangeTrackingEntity
    {
        public DateTime Start { get; set; }
        public RoundStatus RoundStatus { get; set; }
    }
}
