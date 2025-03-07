using System;
using System.Collections.Generic;

namespace identity.Core.Domain.Entities;

public partial class RoleParentChild
{
    public int Id { get; set; }

    public int? ParentRoleId { get; set; }

    public int? ChildRoleId { get; set; }

    public virtual Role? ChildRole { get; set; }

    public virtual Role? ParentRole { get; set; }
}
