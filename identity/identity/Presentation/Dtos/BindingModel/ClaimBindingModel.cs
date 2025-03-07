namespace identity.Presentation.Models.BindingModel
{
    public class ClaimBindingModel
    {
        public int? ClaimId { get; set; }
        public int? Id { get; set; }
        public bool? IsRevoked { get; set; }
        public List<int>? Years { get; set; }
        public int? Year { get; set; }
        public bool? Delete { get; set; }
    }
}
