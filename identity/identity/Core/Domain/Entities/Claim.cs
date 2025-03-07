using System;
using System.Collections.Generic;

namespace identity.Core.Domain.Entities;

public partial class Claim
{
    public int Id { get; set; }

    public string FaName { get; set; } = null!;

    public int? ParentId { get; set; }

    public bool? IsActive { get; set; }

    public string? Verb { get; set; }

    public string? Path { get; set; }

    public string? EnName { get; set; }

    public bool? IsCommon { get; set; }

    public virtual ICollection<Accessibility> Accessibilities { get; set; } = new List<Accessibility>();

    public virtual ICollection<Claim> InverseParent { get; set; } = new List<Claim>();

    public virtual Claim? Parent { get; set; }

    public virtual ICollection<UserRoleClaim> UserRoleClaims { get; set; } = new List<UserRoleClaim>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
