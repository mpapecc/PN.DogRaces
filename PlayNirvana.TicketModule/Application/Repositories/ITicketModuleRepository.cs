using PlayNirvana.CommonModule.DataContext.BaseEntities;
using PlayNirvana.CommonModule.DataContext;

namespace PlayNirvana.TicketModule.Application.Repositories
{
    public interface ITicketModuleRepository<T> :IRepository<T> where T : BaseEntity
    {
    }
}
