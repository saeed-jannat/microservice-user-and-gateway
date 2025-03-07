namespace identity.Presentation.Models.BindingModel
{
    public class CreateClaimBindingModel
    {

        public int Id { get; set; }
        public string FaName { get; set; }
        public string EnName { get; set; }
        public int? ParentId { get; set; }
        public string Path { get; set; }
        public bool IsActive { get; set; }
        public string Verb { get; set; }
    }
}
