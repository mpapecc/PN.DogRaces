using Microsoft.EntityFrameworkCore;
using PlayNirvana.CommonModule.DataContext;
using PlayNirvana.CommonModule.SharedEntites;
using PlayNirvana.TicketModule.Domain.Entites;

namespace PlayNirvana.TicketModule.Infrastructure.DataContext
{
    public class TicketModuleDbContext : PlayNirvanaDbContext
    {
        public TicketModuleDbContext(DbContextOptions<TicketModuleDbContext> options) 
            : base(options, schema : DbSchema.Tickets)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RaceDogResult>().ToTable(x => x.ExcludeFromMigrations()).Metadata.SetSchema(DbSchema.Rounds);

            base.OnModelCreating(modelBuilder);
        }

        DbSet<Ticket> Tickets { get; set; }
        DbSet<Bet> Bets { get; set; }
        DbSet<DogPosition> DogPositions { get; set; }
        DbSet<RaceDogResult> RaceDogResults { get; set; }
    }
}
