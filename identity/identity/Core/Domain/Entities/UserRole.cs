using System;
using System.Collections.Generic;

namespace identity.Core.Domain.Entities;

public partial class UserRole
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? RoleId { get; set; }

    public int? OrganizationId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual Role? Role { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<UserRoleClaim> UserRoleClaims { get; set; } = new List<UserRoleClaim>();

    public virtual ICollection<Year> Years { get; set; } = new List<Year>();
}
