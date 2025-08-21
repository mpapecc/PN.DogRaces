using PlayNirvana.Common.DataContext;
using PlayNirvana.Common.DataContext.BaseEntities;

namespace PlayNirvana.RoundModule.Application.Repositories
{
    public interface IRoundModuleRepository<T> : IRepository<T> where T : BaseEntity
    {
    }
}
