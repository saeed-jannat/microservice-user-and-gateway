namespace identity.Presentation.Models.ResourceModel
{
    public class CoockieClaimResourceModel
    {
        public int Id { get; set; }
        public bool IsRevoked { get; set; }
        public List<int>? Years { get; set; }
    }
}
