namespace NPI.Data.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
}

public abstract class AuditableEntity : BaseEntity
{
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
}
