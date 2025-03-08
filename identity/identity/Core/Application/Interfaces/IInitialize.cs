using identity.Infrastructure.Data;

namespace identity.Core.Application.Interfaces
{
    interface IInitialize
    {
        public Task SendClaimToApiGAteway();
    }
}
