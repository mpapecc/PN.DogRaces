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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        DbSet<Round> Rounds { get; set; }
        DbSet<RaceDogResult> RaceDogResults { get; set; }
    }
}
