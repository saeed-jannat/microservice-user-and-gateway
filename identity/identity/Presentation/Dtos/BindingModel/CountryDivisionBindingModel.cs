namespace identity.Presentation.Models.BindingModel
{
    public class CountryDivisionBindingModel
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public bool? IsActive { get; set; }

        public string? Name { get; set; }

        public bool? IsState { get; set; }

        public bool? IsCity { get; set; }
    }
}
