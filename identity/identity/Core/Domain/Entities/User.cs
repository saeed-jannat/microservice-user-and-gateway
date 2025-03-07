using System;
using System.Collections.Generic;

namespace identity.Core.Domain.Entities;

public partial class User
{
    public int Id { get; set; }

    public string? UserName { get; set; }

    public string PassWord { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? NationalCode { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Description { get; set; }

    public DateTime? SubscriptionEndDate { get; set; }

    public bool? Status { get; set; }

    public bool Sex { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UserId { get; set; }

    public long? ZoneId { get; set; }

    public virtual ICollection<UserParentChild> UserParentChildChildUsers { get; set; } = new List<UserParentChild>();

    public virtual ICollection<UserParentChild> UserParentChildParentUsers { get; set; } = new List<UserParentChild>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual Zone? Zone { get; set; }
}
