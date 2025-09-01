using PlayNirvana.RoundModule.Domain.Entites;

namespace PlayNirvana.RoundModule.Application.Repositories
{
    public interface IRoundRepository : IRoundModuleRepository<Round>
    {
        DateTime GetLastRoundStart();
        IQueryable<Round> LockedRoundQuery();
        IQueryable<Round> RoundsForProcessQuery();
    }
}
