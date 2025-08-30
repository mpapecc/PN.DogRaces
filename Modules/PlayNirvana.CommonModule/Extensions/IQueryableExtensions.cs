using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using PlayNirvana.CommonModule.DataContext.BaseEntities;
using System.Linq.Expressions;

namespace PlayNirvana.CommonModule.Extensions
{
    public static class IQueryableExtensions
    {
        public static int ExecuteUpdateWithChangeTracking<T>(this IQueryable<T> source, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setProps)
            where T : BaseChangeTrackingEntity
        {
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> baseExpr = s => s.SetProperty(x => x.UpdatedOn, DateTime.UtcNow);

            var replaceVisitor = new ReplaceExpressionVisitor(setProps.Parameters[0], baseExpr.Body);
            var newBody = replaceVisitor.Visit(setProps.Body)!;
            var expres = Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(newBody, baseExpr.Parameters);

            return source.ExecuteUpdate(expres);
        }

        private sealed class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression from;
            private readonly Expression to;
            public ReplaceExpressionVisitor(Expression from, Expression to) 
            {
                this.from = from;
                this.to = to; 
            }

            public override Expression Visit(Expression? node)
            {
                return node == this.from ? this.to : base.Visit(node);
            }
        }
    }
}
