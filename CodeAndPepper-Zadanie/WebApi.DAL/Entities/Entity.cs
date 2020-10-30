namespace WebApi.DAL.Entities
{
    public abstract class Entity
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
