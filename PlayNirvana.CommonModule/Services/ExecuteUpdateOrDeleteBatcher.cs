using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace PlayNirvana.CommonModule.Services
{
    public interface IExecuteUpdateOrDeleteBatcher
    {
        void ExecuteUpdateOrDeleteInBatch<T>(int batchSize, IQueryable<T> query, Func<IQueryable<T>, int> updateOrDeleteFunction);
    }

    public class ExecuteUpdateOrDeleteBatcher : IExecuteUpdateOrDeleteBatcher
    {
        public void ExecuteUpdateOrDeleteInBatch<T>(
            int batchsize,
            IQueryable<T> query,
            Func<IQueryable<T>, int> updateOrDeleteFunction
            )
        {
            var queryForUpdateOrDelete = query
                    .Take(batchsize);

            int updatedOrDeleteCount = 0;

            do
            {
                updatedOrDeleteCount = updateOrDeleteFunction(query);
            }
            while (updatedOrDeleteCount == batchsize);
        }
    }
}
