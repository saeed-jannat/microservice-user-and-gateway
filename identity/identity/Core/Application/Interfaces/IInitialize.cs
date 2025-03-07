using identity.Infrastructure.Data;

namespace identity.Core.Application.Interfaces
{
    public interface IInitialize
    {
        public Task SendClaimToApiGAteway();
    }
}
