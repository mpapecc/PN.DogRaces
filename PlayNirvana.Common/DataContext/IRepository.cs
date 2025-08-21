using PlayNirvana.Common.DataContext.BaseEntities;

namespace PlayNirvana.Common.DataContext
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> Query();
        void Insert(T entity);
        void InsertRange(IEnumerable<T> entities);
        int Commit();
    }
}
