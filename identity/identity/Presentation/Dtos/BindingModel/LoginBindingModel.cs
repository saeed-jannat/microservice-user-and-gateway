namespace identity.Presentation.Models.BindingModel
{
    public class LoginBindingModel
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public int RoleId { get; set; }
        public int OrganizationId { get; set; }
    }
}
