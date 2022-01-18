using System;

namespace MyHealthNotebook.Entities.DbSet
{
    public abstract class BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Status { get; set; } = 1;
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdateDate { get; set; }
    }
}