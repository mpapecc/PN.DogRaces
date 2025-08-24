namespace PlayNirvana.CommonModule.DataContext.BaseEntities
{
    public abstract class BaseChangeTrackingEntity : BaseEntity
    {
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
