namespace identity.Presentation.Models.BindingModel
{
    public class RoleClaimBindingModel
    {
        public int roleId { get; set; }
        public List<int> claims { get; set; }
    }
}
