namespace api_gateway.Core.Application.DTOs
{
    public class Claim
    {
        public required int id { get; set; }
        public required string faName { get; set; } = null!;
        public required string enName { get; set; }
        public int? parentId { get; set; }
        public required string path { get; set; }
        public required string verb { get; set; }
        public required bool isActive { get; set; }
        public bool isCommon { get; set; }
        public List<Claim>? inverseParent { get; set; }
    }
}
