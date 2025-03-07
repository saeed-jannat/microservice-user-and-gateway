namespace identity.Presentation.Models.BindingModel
{
    public class UserBindingModel
    {
        public int Id { get; set; }
        public string? UserName { get; set; }

        public string PassWord { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? NationalCode { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Description { get; set; }

        public DateTime? SubscriptionEndDate { get; set; }

        public bool? Status { get; set; }
        public bool Sex { get; set; }
    }
}
