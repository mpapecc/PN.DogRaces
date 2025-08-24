using PlayNirvana.CommonModule.DataContext.BaseEntities;
using PlayNirvana.CommonModule.DataContext.Repositories;

namespace PlayNirvana.RoundModule.Application.Repositories
{
    public interface IRoundModuleRepository<T> : IRepository<T> where T : BaseEntity
    {
    }
}
