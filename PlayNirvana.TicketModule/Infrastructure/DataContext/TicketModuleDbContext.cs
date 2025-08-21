using Microsoft.EntityFrameworkCore;
using PlayNirvana.CommonModule.DataContext;
using PlayNirvana.TicketModule.Domain.Entites;

namespace PlayNirvana.TicketModule.Infrastructure.DataContext
{
    public class TicketModuleDbContext : PlayNirvanaDbContext
    {
        public TicketModuleDbContext(DbContextOptions<TicketModuleDbContext> options) : base(options)
        {
        }

        DbSet<Ticket> Tickets { get; set; }
        DbSet<Bet> Bets { get; set; }
        DbSet<DogPosition> DogPositions { get; set; }
    }
}
