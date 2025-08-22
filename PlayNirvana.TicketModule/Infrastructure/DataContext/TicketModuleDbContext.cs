using Microsoft.EntityFrameworkCore;
using PlayNirvana.CommonModule.DataContext;
using PlayNirvana.TicketModule.Domain.Entites;

namespace PlayNirvana.TicketModule.Infrastructure.DataContext
{
    public class TicketModuleDbContext : PlayNirvanaDbContext
    {
        public static readonly string schema = "tickets";
        public TicketModuleDbContext(DbContextOptions<TicketModuleDbContext> options) 
            : base(options, schema : schema)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RaceDogResult>().ToTable(x => x.ExcludeFromMigrations()) ;
            base.OnModelCreating(modelBuilder);
        }

        DbSet<Ticket> Tickets { get; set; }
        DbSet<Bet> Bets { get; set; }
        DbSet<DogPosition> DogPositions { get; set; }
        DbSet<RaceDogResult> RaceDogResults { get; set; }
    }
}
