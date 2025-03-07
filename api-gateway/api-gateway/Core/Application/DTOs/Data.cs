namespace api_gateway.Core.Application.DTOs
{
    public class Data<T>
    {
        public string? message { get; set; }
        public List<T>? data { get; set; }
    }
}
