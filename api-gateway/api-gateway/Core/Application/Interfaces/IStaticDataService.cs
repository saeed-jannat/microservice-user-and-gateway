using api_gateway.Core.Application.DTOs;

namespace api_gateway.Core.Application.Interfaces
{
    public interface IStaticDataService
    {
        Task<string> InitializeStaticDataService();
        void Role(Role role, bool? createEdit,int? delete);
        void Claim(Claim claim, int roleId);
        //public void GetAllControllersAndActions();
        void SetPublicKey(string publickey);
        Task<string> GetAllTheClaims();
    }
}
