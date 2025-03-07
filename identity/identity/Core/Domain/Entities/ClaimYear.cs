using System;
using System.Collections.Generic;

namespace identity.Core.Domain.Entities;

public partial class ClaimYear
{
    public int ClaimId { get; set; }

    public int YearId { get; set; }

    public virtual Claim Claim { get; set; } = null!;

    public virtual Year Year { get; set; } = null!;
}
