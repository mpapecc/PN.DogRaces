using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PlayNirvana.CommonModule.DataContext.BaseEntities;

namespace PlayNirvana.CommonModule.DataContext.Interceptors
{
    public class ChangeTrackingEntityInterceptor : ISaveChangesInterceptor
    {
        public virtual InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context == null)
                return result;

            foreach (var entry in eventData.Context.ChangeTracker.Entries())
            {
                if (entry.Entity is BaseChangeTrackingEntity addedEntity && entry.State == EntityState.Added)
                {
                    addedEntity.CreatedOn = DateTime.UtcNow;
                    addedEntity.UpdatedOn = DateTime.UtcNow;
                }

                if (entry.Entity is BaseChangeTrackingEntity modifiedEntity && entry.State == EntityState.Modified)
                {
                    modifiedEntity.UpdatedOn = DateTime.UtcNow;
                }
            }

            return result;
        }
    }
}
