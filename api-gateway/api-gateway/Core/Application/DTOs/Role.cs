namespace api_gateway.Core.Application.DTOs
{
    public class Role
    {
        public required int id { get; set; }
        public required string name { get; set; }
        public int? zoneId { get; set; }
        public required bool selfRegister { get; set; }
        public required bool needsApproval { get; set; }
        public required List<Claim> claims { get; set; }
    }
}
