using identity.Core.Domain.Entities;
using System.Security.Claims;

namespace identity.Core.Application.Interfaces
{
    public interface IToken
    {
        public Task<string> BuildToken(string username, string pass, int roleid, int zoneid, User user);
        public ClaimsPrincipal ValidationToken(string token);
    }
}
