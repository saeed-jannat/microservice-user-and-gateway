using System.Net;
using System.Text;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using Newtonsoft.Json;
using api_gateway.Core.Common.Utilities;

public class AdminPageAggregator : IDefinedAggregator
{
    public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
    {
        if (responses == null || responses.Count == 0)
        {
            return new DownstreamResponse(
                new StringContent(JsonConvert.SerializeObject(new
                {
                    status = 500,
                    message = "error",
                    data = (object)null,
                    errors = new List<string> { "سرویس در دسترس نیست. لطفاً بعداً تلاش کنید." },
                    warning = new List<string>()
                }), Encoding.UTF8, "application/json"),
                HttpStatusCode.ServiceUnavailable,
                new List<KeyValuePair<string, IEnumerable<string>>>(),
                "Service Unavailable"
            );
        }
        List<string> errors = new List<string>();
        var responseData = new Dictionary<string, object>();
        string[] requestNames = { "نقش ها", "دسترسی ها", "کاربر ها", "سازمان ها", "استان ها" };
        List<int> statusOfRequest = new List<int>();
        for (int i = 0; i < responses.ToArray().Length; i++)
        {
            try
            {
                var content = await DownStream.GetResponseContent(responses, i, new List<string> { "data" });
                if (content is null || (content is IEnumerable<object> list && !list.Any()) || content.Count==0)
                {
                    errors.Add($"بخش '{requestNames[i]}' در دسترس نیست.");
                    continue; // این درخواست را اضافه نکن
                }
                var s = Convert.ToInt64(content[1]);
                if (s == 200)
                {
                    responseData[responses[i].Items.DownstreamRoute().Key] = content[0];
                }
                else
                {
                    string error = Convert.ToString(content[0]);
                    var t = error?.Split("\"");
                    errors.Add(t?.Count() > 1 ? t[1] : "");
                }
                
                var statusCode = Convert.ToInt64(content[1]);
                statusOfRequest.Add((int)statusCode);
            }
            catch (Exception ex)
            {
                errors.Add($"خطا در پردازش '{requestNames[i]}': {ex.Message}");
            }
        }
        int status = statusOfRequest.GroupBy(n => n)
                                  .OrderByDescending(g => g.Count())
                                  .First()
                                  .Key;
        var aggregatedContent = new
        {
            status = status,
            message = status switch
            {
                200 => "success",
                400 => "bad request",
                404 => "not found",
                500 => "internal server error",
            },
            data = new { message = "", data = responseData },
            errors = errors.Distinct().Where(x => x != ""),
            warning = new List<string>()
        };
        var stringContent = new StringContent(JsonConvert.SerializeObject(aggregatedContent), Encoding.UTF8, "application/json");
        return new DownstreamResponse(stringContent, status == 200 ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, new List<KeyValuePair<string, IEnumerable<string>>>(), "OK");
    }
}
