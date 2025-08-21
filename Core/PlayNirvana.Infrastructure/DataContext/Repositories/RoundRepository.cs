using Microsoft.EntityFrameworkCore;
using PlayNirvana.Bll.Repositories;
using PlayNirvana.Domain.Entites;
using PlayNirvana.Shared.Enums;

namespace PlayNirvana.Infrastructure.DataContext.Repositories
{
    public class RoundRepository : BaseRepository<Round>, IRoundRepository
    {
        private readonly PlayNirvanaDbContext context;

        public RoundRepository(PlayNirvanaDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Sp_TranslateActiveAndIdleRoundsStartInFuture()
        {
            this.context.Database.ExecuteSql($"EXECUTE dbo.sproc_TranslateActiveAndIdleRoundsStartInFuture");
        }

        public int GetIdleRoundsCount()
        {
            return IdleRoundQuery().Count();
        }

        public int GetActiveRoundsCount()
        {
            return ActiveRoundQuery().Count();
        }

        public DateTime GetLastIdleRoundStart()
        {
            return IdleRoundQuery()
                .OrderByDescending(x => x.Start)
                .Select(x => x.Start)
                .FirstOrDefault();
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

        public IQueryable<Round> GetNextRoundForExecutionQuery()
        {
            return ActiveRoundQuery()
                .OrderBy(x => x.Start)
                .Take(1);
        }

        public IQueryable<Round> ActiveAndIdleRoundQuery()
        {
            return base.Query().Where(x => x.RoundStatus == RoundStatus.Active || x.RoundStatus == RoundStatus.Idle);
        }
    }
}
