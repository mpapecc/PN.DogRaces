using PlayNirvana.Bll.Models;
using PlayNirvana.Domain.Entites;

namespace PlayNirvana.Bll.Repositories
{
    public interface IRoundRepository: IRepository<Round>
    {
        int GetIdleRoundsCount();
        int GetActiveRoundsCount();
        DateTime GetLastIdleRoundStart();
        IQueryable<Round> IdleRoundQuery();
        IQueryable<Round> GetNextRoundForActivationQuery();
        IQueryable<Round> LockedRoundQuery();
        IQueryable<Round> ActiveRoundQuery();
    }
}
