using PlayNirvana.CommonModule.DataContext.BaseEntities;

namespace PlayNirvana.CommonModule.DataContext
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> Query();
        void Insert(T entity);
        void InsertRange(IEnumerable<T> entities);
        int Commit();
    }
}
