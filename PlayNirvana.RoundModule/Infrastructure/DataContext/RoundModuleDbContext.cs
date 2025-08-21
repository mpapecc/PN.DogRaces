using Microsoft.EntityFrameworkCore;
using PlayNirvana.CommonModule.DataContext;
using PlayNirvana.RoundModule.Domain.Entites;

namespace PlayNirvana.RoundModule.Infrastructure.DataContext
{
    public class RoundModuleDbContext : PlayNirvanaDbContext
    {
        public static readonly string schema = "rounds";
        public RoundModuleDbContext(DbContextOptions<RoundModuleDbContext> options) 
            : base(options, schema)
        {
        }

        DbSet<Round> Rounds { get; set; }
        DbSet<RaceDogResult> RaceDogResults { get; set; }
    }
}
