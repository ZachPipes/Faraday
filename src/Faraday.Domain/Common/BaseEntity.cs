// src/Faraday.Domain/BaseEntity.cs

namespace Faraday.Domain.Common;

public abstract class BaseEntity {
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Mark this entity as modified (updates the ModifiedAt timestamp)
    /// </summary>
    protected void MarkAsModified() {
        ModifiedAt = DateTime.UtcNow;
    }
}