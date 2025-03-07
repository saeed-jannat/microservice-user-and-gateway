using System;
using System.Collections.Generic;

namespace identity.Core.Domain.Entities;

public partial class Organization
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? NationalCode { get; set; }

    public string? MemoryId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public long? ZoneId { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual Zone? Zone { get; set; }
}
