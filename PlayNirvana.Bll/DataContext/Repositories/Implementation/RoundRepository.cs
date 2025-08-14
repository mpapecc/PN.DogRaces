using Microsoft.EntityFrameworkCore;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Enums;

namespace PlayNirvana.Bll.DataContext.Repositories.Implementation
{
    public class RoundRepository : BaseRepository<Round>
    {
        public RoundRepository(PlayNirvanaDbContext context) : base(context)
        {
        }

        public int GetIdleRoundsCount()
        {
            return IdleRoundQuery().Count();
        }

        public DateTime GetLastIdleRoundStartDate()
        {
            return IdleRoundQuery()
                .OrderByDescending(x => x.Start)
                .Select(x => x.Start)
                .FirstOrDefault();
        }

        public IQueryable<Round> GetNextNIdleRounds(int roundsNumber)
        {
            return IdleRoundQuery()
                .OrderBy(x => x.Start)
                .Take(roundsNumber);
        }

        public void ActivateNextNRounds(int roundsNumber = 1)
        {
            GetNextNIdleRounds(roundsNumber)
                .ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus , RoundStatus.Active));
        }

        public void LockNextActiveRoundForBets(int roundsNumber = 1)
        {
            // maybe we can check when placing bet if there is less then 5 seconds before start 
            // and prevent bet...in that way we are reducing database updates but we have to make sure we 
            // are validating in code in every place!!!
            GetNextRoundForActivationQuery(roundsNumber)
                .ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Locked));
        }

        public void StartLockedRound(int roundsNumber = 1)
        {
            LockedRoundQuery()
                .OrderBy(x => x.Start)
                .Take(roundsNumber)
                .ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.InProgress));
        }

        public void FinishInProgressRound(int roundsNumber = 1)
        {
            InProgressRoundQuery()
                .OrderBy(x => x.Start)
                .Take(roundsNumber)
                .ExecuteUpdate(s => s.SetProperty(x => x.RoundStatus, RoundStatus.Finished));
        }

        public IQueryable<Round> IdleRoundQuery()
        {
            return base.Query().Where(x => x.RoundStatus == RoundStatus.Idle);
        }

        public IQueryable<Round> ActiveRoundQuery()
        {
            return base.Query().Where(x => x.RoundStatus == RoundStatus.Active);
        }

        public IQueryable<Round> LockedRoundQuery()
        {
            return base.Query().Where(x => x.RoundStatus == RoundStatus.Locked);
        }

        public IQueryable<Round> InProgressRoundQuery()
        {
            return base.Query().Where(x => x.RoundStatus == RoundStatus.InProgress);
        }

        public IQueryable<Round> GetNextRoundForActivationQuery(int roundsNumber)
        {
            return ActiveRoundQuery()
                .OrderBy(x => x.Start)
                .Take(roundsNumber);
        }
    }
}
