namespace api_gateway.Core.Application.DTOs
{
    public static class StaticData
    {
        public  static  List<Role>? Roles { get; set; }
        public static List<Action>? Actions { get; set; }
        public static List<string>? Controller { get; set; }
        public static List<Claim>? Claims { get; set; }
        public static string? PublicKey { get; set; }
        public static string? IdentityBaseUrl { get; set; }
    }
}
