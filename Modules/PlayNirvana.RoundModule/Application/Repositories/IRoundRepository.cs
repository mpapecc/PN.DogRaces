using PlayNirvana.RoundModule.Domain.Entites;

namespace PlayNirvana.RoundModule.Application.Repositories
{
    public interface IRoundRepository : IRoundModuleRepository<Round>
    {
        void Sp_TranslateActiveAndIdleRoundsStartInFuture();
        int GetIdleRoundsCount();
        int GetActiveRoundsCount();
        DateTime GetLastIdleRoundStart();
        IQueryable<Round> IdleRoundQuery();
        IQueryable<Round> GetNextRoundForExecutionQuery();
        IQueryable<Round> LockedRoundQuery();
        IQueryable<Round> ActiveRoundQuery();
        IQueryable<Round> ActiveAndIdleRoundQuery();
    }
}
