namespace api_gateway.Core.Application.DTOs
{
    public class Action
    {
        public required string Name { get; set; }
        public required string Verb { get; set; }
        public required string Controller { get; set; }
        public int? Id { get; set; }
    }
}
