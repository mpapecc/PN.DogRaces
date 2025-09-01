using PlayNirvana.CommonModule.DataContext.BaseEntities;
using PlayNirvana.CommonModule.DataContext.Repositories;
using PlayNirvana.RoundModule.Application.Repositories;

namespace PlayNirvana.RoundModule.Infrastructure.DataContext
{
    public class RoundModuleRepository<T> : BaseRepository<T>, IRoundModuleRepository<T> where T : BaseEntity
    {
        public RoundModuleRepository(RoundModuleDbContext context) : base(context)
        {
        }
    }
}
