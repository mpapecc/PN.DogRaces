using PlayNirvana.Domain.Entites.BaseEntities;

namespace PlayNirvana.Bll.DataContext.Repositories.Abstraction
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> Query();
        void Insert(T entity);
        void InsertRange(IEnumerable<T> entities);
        int Commit();
    }
}
