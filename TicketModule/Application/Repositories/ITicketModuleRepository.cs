using PlayNirvana.Common.DataContext.BaseEntities;
using PlayNirvana.Common.DataContext;

namespace PlayNirvana.TicketModule.Application.Repositories
{
    public interface ITicketModuleRepository<T> :IRepository<T> where T : BaseEntity
    {
    }
}
