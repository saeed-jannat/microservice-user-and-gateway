using System;
using System.Collections.Generic;

namespace identity.Core.Domain.Entities;

public partial class Accessibility
{
    public int Id { get; set; }

    public string? Action { get; set; }

    public string? Controller { get; set; }

    public string FaActionName { get; set; } = null!;

    public int? ClaimId { get; set; }

    public virtual Claim? Claim { get; set; }
}
