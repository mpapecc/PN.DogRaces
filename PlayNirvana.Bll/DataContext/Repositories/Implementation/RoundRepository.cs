using Microsoft.EntityFrameworkCore;
using PlayNirvana.Bll.Models;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Enums;

namespace PlayNirvana.Bll.DataContext.Repositories.Implementation
{
    public class RoundRepository : BaseRepository<Round>
    {
        public RoundRepository(PlayNirvanaDbContext context) : base(context)
        {
        }

        public IEnumerable<RoundModel> GetActiveRounds()
        {
            return ActiveRoundQuery()
                .Select(x => new RoundModel()
                {
                    RoundStatus = x.RoundStatus,
                    Start = x.Start
                })
                .ToList();
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

        public IQueryable<Round> GetNextRoundForActivationQuery()
        {
            return ActiveRoundQuery()
                .OrderBy(x => x.Start)
                .Take(1);
        }
    }
}
