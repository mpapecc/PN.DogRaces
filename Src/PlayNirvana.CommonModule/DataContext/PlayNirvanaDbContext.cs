using Microsoft.EntityFrameworkCore;
using PlayNirvana.CommonModule.DataContext.Interceptors;

namespace PlayNirvana.CommonModule.DataContext
{
    public abstract class PlayNirvanaDbContext : DbContext
    {
        private readonly string schema;

        public PlayNirvanaDbContext(DbContextOptions options, string schema) : base(options)
        {
            this.schema = schema;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(new ChangeTrackingEntityInterceptor());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(this.schema);
        }
    }
}
