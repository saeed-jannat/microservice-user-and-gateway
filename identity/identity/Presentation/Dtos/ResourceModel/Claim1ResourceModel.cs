namespace identity.Presentation.Models.ResourceModel
{
    public class Claim1ResourceModel
    {
        public int Id { get; set; }

        public string FaName { get; set; } = null!;

        public string? EnName { get; set; }

        public int? ParentId { get; set; }
        public string? Path { get; set; }
        public string? Verb { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsCommon { get; set; }

        public virtual IEnumerable<Claim1ResourceModel> InverseParent { get; set; } = new List<Claim1ResourceModel>();

    }
}
