using Microsoft.EntityFrameworkCore;

namespace PlayNirvana.Common.DataContext
{
    public abstract class PlayNirvanaDbContext : DbContext
    {
        public PlayNirvanaDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
