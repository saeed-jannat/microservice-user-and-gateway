namespace api_gateway.Core.Application.DTOs
{
    public class DTO<T>
    {
        public required int status { get; set; }
        public required string message { get; set; }
        public required Data<T> data { get; set; }
        public required List<string> errors { get; set; }
        public required List<string> warning { get; set; }
    }
}
