using System;
using System.Collections.Generic;

namespace identity.Core.Domain.Entities;

public partial class Year
{
    public int Id { get; set; }

    public int? Value { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<UserRoleClaim> UserRoleClaims { get; set; } = new List<UserRoleClaim>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
