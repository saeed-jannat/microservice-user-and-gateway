using System;
using System.Collections.Generic;

namespace identity.Core.Domain.Entities;

public partial class UserRoleClaim
{
    public int Id { get; set; }

    public int? UserRoleId { get; set; }

    public int? ClaimsId { get; set; }

    public bool? IsRevoked { get; set; }

    public virtual Claim? Claims { get; set; }

    public virtual UserRole? UserRole { get; set; }

    public virtual ICollection<Year> Years { get; set; } = new List<Year>();
}
