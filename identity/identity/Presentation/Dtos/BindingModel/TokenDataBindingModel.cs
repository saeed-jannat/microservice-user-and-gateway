namespace identity.Presentation.Models.BindingModel
{
    public class TokenDataBindingModel
    {
        public string userid { get; set; }
        public string username { get; set; }
        public string pass { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string nationalcode { get; set; }
        public string sex { get; set; }
        public string role { get; set; }
        public List<int> add_claim { get; set; }
        public List<int> remove_claim { get; set; }
    }
}
