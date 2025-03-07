using api_gateway.Core.Application.DTOs;

namespace api_gateway.Core.Application.Interfaces
{
    public interface IStaticDataService
    {
        public Task<string> InitializeStaticDataService();
        public void Role(Role role, bool? createEdit,int? delete);
        public void Claim(Claim claim, int roleId);
        //public void GetAllControllersAndActions();
        public void SetPublicKey(string publickey);
        public Task<string> GetAllTheClaims();
    }
}
