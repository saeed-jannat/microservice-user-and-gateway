using System.Security.Claims;

namespace api_gateway.Core.Application.Interfaces
{
    interface ICookie
    {
        public ClaimsPrincipal? ValidationToken(string token);
    }
}
