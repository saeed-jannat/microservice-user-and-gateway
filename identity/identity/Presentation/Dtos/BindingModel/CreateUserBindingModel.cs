namespace identity.Presentation.Models.BindingModel
{
    public class CreateUserBindingModel
    {
        public int? OrganizationId { get; set; }
        public int? Id { get; set; }
        public long? ZoneId { get; set; }
        public string? UserName { get; set; }
        public string? PassWord { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NationalCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public string? SubscriptionEndDate { get; set; }
        public bool? Status { get; set; }
        public bool? Sex { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public DateTime? StartDate { get; set; }
        public List<UserRoleBindingModel?>? UserRoles { get; set; }
        public List<ClaimBindingModel?>? ExtraClaim { get; set; }
    }
}
