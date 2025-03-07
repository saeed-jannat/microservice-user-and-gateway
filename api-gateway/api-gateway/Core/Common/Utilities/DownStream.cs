using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Request.Middleware;
using System.Text.Json;

namespace api_gateway.Core.Common.Utilities
{
    public static class DownStream
    {
        public static async Task<List<object>> GetResponseContent(List<HttpContext> responses, int index, List<string> fieldsToKeep)
        {
            DownstreamRequest t =(DownstreamRequest)responses[index].Items["DownstreamRequest"];
            string token = responses[index].Request.Headers["Authorization"];
            bool? x = Authorization.IsAuthorized(token, t.AbsolutePath.ToLower(), t.Method.ToLower());
            if (Authorization.IsAuthorized(token, t.AbsolutePath.ToLower(), t.Method.ToLower()))
            {
                if (index >= responses.Count || responses[index]?.Items.DownstreamResponse() == null)
                {
                    return new List<object>();
                }

                var jsonString = await responses[index].Items.DownstreamResponse().Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    return new List<object>();
                }

                var fullData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

                if (fullData == null)
                {
                    return new List<object>();
                }
                var status = Convert.ToInt64(fullData["status"]);
                if (status != 200)
                {
                    return new List<object>()
                {
                    fullData["errors"],
                    status
                };
                }
                return new List<object>()
                {
                    fullData.Where(kvp => fieldsToKeep.Contains(kvp.Key)).Select(x => x.Value)
                           .SingleOrDefault(),
                    status
                };
            }
            else
            {
                return new List<object>()
                {
                    (object)"شما به این بخش دسترسی ندارید",
                    401
                };
            }
            
        }
    }
}
