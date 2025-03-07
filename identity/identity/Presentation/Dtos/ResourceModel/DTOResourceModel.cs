namespace identity.Presentation.Models.ResourceModel
{
    public class DTOResourceModel
    {
        public int status { get; set; }
        public string message { get; set; }
        public DataResourceModel data { get; set; }
        public List<string> errors { get; set; }
        public List<string> warning { get; set; }
    }
}
