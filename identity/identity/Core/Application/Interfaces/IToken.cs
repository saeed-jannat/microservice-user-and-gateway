using identity.Core.Domain.Entities;
using System.Security.Claims;

namespace identity.Core.Application.Interfaces
{
    public interface IToken
    {
        Task<string> BuildToken(string username, string pass, int roleid, int zoneid, User user);
        ClaimsPrincipal ValidationToken(string token);
    }
}
