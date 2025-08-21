using PlayNirvana.Bll.Models;
using PlayNirvana.Domain.Entites;

namespace PlayNirvana.Bll.Repositories
{
    public interface IRoundRepository: IRepository<Round>
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
