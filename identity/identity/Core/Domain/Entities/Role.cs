using System;
using System.Collections.Generic;

namespace identity.Core.Domain.Entities;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? SelfRegister { get; set; }

    public bool? NeedsApproval { get; set; }

    public virtual ICollection<RoleParentChild> RoleParentChildChildRoles { get; set; } = new List<RoleParentChild>();

    public virtual ICollection<RoleParentChild> RoleParentChildParentRoles { get; set; } = new List<RoleParentChild>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();
}
