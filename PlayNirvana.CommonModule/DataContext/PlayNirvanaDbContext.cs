using Microsoft.EntityFrameworkCore;

namespace PlayNirvana.CommonModule.DataContext
{
    public abstract class PlayNirvanaDbContext : DbContext
    {
        private readonly string schema;

        public PlayNirvanaDbContext(DbContextOptions options, string schema) : base(options)
        {
            this.schema = schema;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(this.schema);
        }
    }
}
