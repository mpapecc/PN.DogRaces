using Microsoft.EntityFrameworkCore;
using PlayNirvana.RoundModule.Application.Repositories;
using PlayNirvana.RoundModule.Common.Enums;
using PlayNirvana.RoundModule.Domain.Entites;

namespace PlayNirvana.RoundModule.Infrastructure.DataContext
{
    public class RoundRepository : RoundModuleRepository<Round>, IRoundRepository
    {
        private readonly RoundModuleDbContext context;

        public RoundRepository(RoundModuleDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Sp_TranslateActiveAndIdleRoundsStartInFuture()
        {
            context.Database.ExecuteSql($"EXECUTE dbo.sproc_TranslateActiveAndIdleRoundsStartInFuture");
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
