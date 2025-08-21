using Microsoft.EntityFrameworkCore;

namespace PlayNirvana.CommonModule.DataContext
{
    public abstract class PlayNirvanaDbContext : DbContext
    {
        public PlayNirvanaDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
