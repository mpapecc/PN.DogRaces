using PlayNirvana.RoundModule.Domain.Entites;

namespace PlayNirvana.RoundModule.Application.Repositories
{
    public interface IRoundRepository : IRoundModuleRepository<Round>
    {
        int GetIdleRoundsCount();
        int GetActiveRoundsCount();
        DateTime GetLastIdleRoundStart();
        IQueryable<Round> IdleRoundQuery();
        IQueryable<Round> LockedRoundQuery();
        IQueryable<Round> ActiveRoundQuery();
        IQueryable<Round> ActiveAnInProgressRoundQuery();
        IQueryable<Round> NonProcessedQuery();
    }
}
