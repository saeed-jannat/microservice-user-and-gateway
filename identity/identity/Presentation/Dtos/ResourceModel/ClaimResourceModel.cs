using identity.Presentation.Models.BindingModel;

namespace identity.Presentation.Models.ResourceModel
{
    public class ClaimResourceModel : ClaimBindingModel
    {
        public string FaName { get; set; }
        public string EnName { get; set; }
        public int UserRoleClaimId { get; set; }
        public int RoleId { get; set; }
        public int OrganizationId { get; set; }
    }
}
