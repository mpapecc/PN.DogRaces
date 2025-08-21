using PlayNirvana.CommonModule.DataContext;
using PlayNirvana.CommonModule.DataContext.BaseEntities;

namespace PlayNirvana.RoundModule.Application.Repositories
{
    public interface IRoundModuleRepository<T> : IRepository<T> where T : BaseEntity
    {
    }
}
