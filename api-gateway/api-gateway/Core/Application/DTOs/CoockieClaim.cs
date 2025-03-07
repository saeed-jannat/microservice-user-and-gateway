namespace api_gateway.Core.Application.DTOs
{
    public class CoockieClaim
    {
        public required int Id { get; set; }
        public required bool IsRevoked { get; set; }
        public required List<int> Years { get; set; }
    }
}
