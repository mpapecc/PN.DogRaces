using PlayNirvana.CommonModule.DataContext;
using PlayNirvana.CommonModule.DataContext.BaseEntities;
using PlayNirvana.TicketModule.Application.Repositories;

namespace PlayNirvana.TicketModule.Infrastructure.DataContext
{
    public class TicketModuleRepository<T> : BaseRepository<T>, ITicketModuleRepository<T> where T : BaseEntity
    {
        public TicketModuleRepository(TicketModuleDbContext context) : base(context)
        {
        }
    }
}
