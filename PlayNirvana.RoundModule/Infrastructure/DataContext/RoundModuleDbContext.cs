using Microsoft.EntityFrameworkCore;
using PlayNirvana.CommonModule.DataContext;
using PlayNirvana.RoundModule.Domain.Entites;

namespace PlayNirvana.RoundModule.Infrastructure.DataContext
{
    public class RoundModuleDbContext : PlayNirvanaDbContext
    {
        public RoundModuleDbContext(DbContextOptions<RoundModuleDbContext> options) 
            : base(options, DbSchema.Rounds)
        {
        }

        DbSet<Round> Rounds { get; set; }
        DbSet<RaceDogResult> RaceDogResults { get; set; }
    }
}
