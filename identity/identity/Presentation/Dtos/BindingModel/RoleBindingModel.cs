namespace identity.Presentation.Models.BindingModel
{
    public class RoleBindingModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public bool? SelfRegister { get; set; }

        public bool? NeedsApproval { get; set; }
        public List<ClaimBindingModel>? Claims { get; set; }
    }
}
