using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Net;
using System.Text;
using api_gateway.Core.Common.Utilities;

namespace api_gateway.Infrastructure.Ocelot.Aggegators
{
    public class BuildingPageAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            if (responses == null || responses.Count == 0)
            {
                return CreateErrorResponse("سرویس در دسترس نیست. لطفاً بعداً تلاش کنید.");
            }
            var errors = new List<string>();
            var responseData = new Dictionary<string, object>();
            string id = responses.FirstOrDefault()?.Request.RouteValues["id"]?.ToString();
            string[] requestNames = {"صاحبان", "مشخصات ملک", "نقشه و کروکی", "مشخصات زمین","استعلام ها", "مشخصات ساختمان", "ناظرین ساختمان"};
            List<int> statusOfRequest = new List<int>();
            for (int i = 0; i < requestNames.Length; i++)
            {
                try
                {
                    var content = await DownStream.GetResponseContent(responses, i, new List<string> { "data" });
                    if (content is null || (content is IEnumerable<object> list && !list.Any()) || content.Count == 0)
                    {
                        errors.Add($"بخش '{requestNames[i]}' در دسترس نیست.");
                        continue;
                    }
                    var s = Convert.ToInt64(content[1]);
                    if (s == 200)
                    {
                        responseData[responses[i].Items.DownstreamRoute().Key] = content[0];
                    }
                    var error = content[0];
                    var t = Convert.ToString(error)?.Split("\"");
                    errors.Add(t?.Count() > 1 ? t[1] : "");
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
                    200=>"success",
                    400=>"bad request",
                    404=>"not found",
                    500=>"fail",
                },
                data = new { message = "", data = responseData },
                errors = errors.Distinct().Where(x=>x!=""),
                warning = new List<string>()
            };
            return CreateJsonResponse(aggregatedContent, status);
        }
        private DownstreamResponse CreateErrorResponse(string errorMessage)
        {
            var errorContent = new
            {
                status = 500,
                message = "error",
                data = (object)null,
                errors = new List<string> { errorMessage },
                warning = new List<string>()
            };
            return CreateJsonResponse(errorContent, 500);
        }
        private DownstreamResponse CreateJsonResponse(object content, int statusCode)
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            return new DownstreamResponse(stringContent, (HttpStatusCode)statusCode, new List<KeyValuePair<string, IEnumerable<string>>>(), "OK");
        }
    }
}
