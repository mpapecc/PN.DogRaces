using PlayNirvana.RoundModule.Application.Repositories;
using PlayNirvana.RoundModule.Common.Enums;
using PlayNirvana.RoundModule.Domain.Entites;

namespace PlayNirvana.RoundModule.Infrastructure.DataContext
{
    public class RoundRepository : RoundModuleRepository<Round>, IRoundRepository
    {
        public RoundRepository(RoundModuleDbContext context) : base(context)
        {
        }

        public DateTime GetLastRoundStart()
        {
            return base.Query()
                .OrderByDescending(x => x.Start)
                .Select(x => x.Start)
                .FirstOrDefault();
        }

        public IQueryable<Round> LockedRoundQuery()
        {
            return base.Query().Where(x => x.RoundStatus == RoundStatus.Locked);
        }

        public IQueryable<Round> RoundsForProcessQuery()
        {
            return base.Query().Where(x => x.RoundStatus == RoundStatus.Active ||  x.RoundStatus == RoundStatus.InProgress);
        }
    }
}
