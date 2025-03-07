using api_gateway.Core.Application.DTOs;
using api_gateway.Core.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace api_gateway.Infrastructure.Ocelot.DelegatingHandlers
{
    public class RoleHandler : DelegatingHandler
    {
        private readonly IServiceProvider _serviceProvider;
        public RoleHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            var m = request.Method.Method;
            string requestBody;
            Role role = null;
            if (m!="DELETE")
            {
                requestBody = await request?.Content?.ReadAsStringAsync();
                role = JsonSerializer.Deserialize<Role>(requestBody);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            }
            var res=await base.SendAsync(request, token);
            if (res.IsSuccessStatusCode)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    IStaticDataService staticDataService = scope.ServiceProvider.GetRequiredService<IStaticDataService>();
                    if (m == "PUT")
                        staticDataService.Role(role, true,null);
                    else if (m == "DELETE")
                    {
                        staticDataService.Role(role, false,Convert.ToInt32(request.RequestUri.Segments[4]));
                    }
                    else
                        staticDataService.Role(role,false,null);
                }
            }
            return res;
        }
    }
}
