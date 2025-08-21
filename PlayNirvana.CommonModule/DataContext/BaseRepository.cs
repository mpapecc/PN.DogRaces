using PlayNirvana.CommonModule.DataContext.BaseEntities;

namespace PlayNirvana.CommonModule.DataContext
{
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly PlayNirvanaDbContext context;

        public BaseRepository(PlayNirvanaDbContext context)
        {
            this.context = context;
        }

        public virtual IQueryable<T> Query()
        {
            return context.Set<T>();
        }

        public virtual void Insert(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public virtual void InsertRange(IEnumerable<T> entities)
        {
            context.Set<T>().AddRange(entities);
        }

        public int Commit()
        {
            return context.SaveChanges();
        }
    }
}
