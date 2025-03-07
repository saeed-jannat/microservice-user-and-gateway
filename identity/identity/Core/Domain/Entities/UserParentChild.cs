using System;
using System.Collections.Generic;

namespace identity.Core.Domain.Entities;

public partial class UserParentChild
{
    public int Id { get; set; }

    public int? ParentUserId { get; set; }

    public int? ChildUserId { get; set; }

    public virtual User? ChildUser { get; set; }

    public virtual User? ParentUser { get; set; }
}
