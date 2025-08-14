using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using PlayNirvana.Domain.Entites;

namespace PlayNirvana.Bll.DataContext
{
    public class PlayNirvanaDbContext: DbContext
    {
        public PlayNirvanaDbContext(DbContextOptions<PlayNirvanaDbContext> options):base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RacingDog>(b =>
            {
                b.HasData(
                    new RacingDog { Id = 1, Name = "Dogo1", Number = 1 },
                    new RacingDog { Id = 2, Name = "Dogo2", Number = 2 },
                    new RacingDog { Id = 3, Name = "Dogo3", Number = 3 },
                    new RacingDog { Id = 4, Name = "Dogo4", Number = 4 },
                    new RacingDog { Id = 5, Name = "Dogo5", Number = 5 },
                    new RacingDog { Id = 6, Name = "Dogo6", Number = 6 },
                    new RacingDog { Id = 7, Name = "Dogo7", Number = 7 },
                    new RacingDog { Id = 8, Name = "Dogo8", Number = 8 },
                    new RacingDog { Id = 9, Name = "Dogo9", Number = 9 }
                );
            });

            base.OnModelCreating(modelBuilder);
        }

        DbSet<Ticket> Tickets { get; set; }
        DbSet<Round> Rounds { get; set; }
        DbSet<Bet> Bets { get; set; }
        DbSet<DogPosition> DogPositions { get; set; }
        DbSet<RacingDog> RacingDogs { get; set; }
        DbSet<RaceDogResult> RaceDogResults { get; set; }
    }
}
