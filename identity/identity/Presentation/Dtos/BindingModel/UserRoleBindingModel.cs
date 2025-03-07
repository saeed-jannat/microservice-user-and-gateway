namespace identity.Presentation.Models.BindingModel
{
    public class UserRoleBindingModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public int? OrganizationId { get; set; }
        public List<ClaimBindingModel>? Claims { get; set; }
        public DateTime? StartDate { get; set; }
        public List<int> Years { get; set; }
        public bool? IsRevoked { get; set; }
    }
}
