using api_gateway.Core.Application.DTOs;
using api_gateway.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;

namespace api_gateway.Core.Application.Services
{
    public class staticDataService : IStaticDataService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public staticDataService(HttpClient httpClient, IConfiguration configuration)
        {
            _client = httpClient;
            _baseUrl = configuration["IdentityBaseUrl"] ?? "";
        }
        public void Claim(Claim claim,int roleId)
        {

            Role? r = StaticData.Roles?.SingleOrDefault(x => x.id == roleId);
            if (r != null)
            {
                Claim? c = r.claims.SingleOrDefault(c => c.id == claim.id);
                if (c == null)
                    r.claims.Add(claim);
                else
                {
                    c.path = claim.path;
                    c.enName = claim.enName;
                    c.faName = claim.faName;
                    c.verb = claim.verb;
                    c.parentId = c.parentId;
                    c.isActive = c.isActive;
                }
            }
        }
        public async Task<string> InitializeStaticDataService()
        {
            try
            {
                var responseRoles = await _client.GetAsync($"{_baseUrl}/api/v1/role");
                DTO<Role>? resRoles = null;
                var jsonStringRoles = await responseRoles.Content.ReadAsStringAsync();
                resRoles = JsonSerializer.Deserialize<DTO<Role>>(jsonStringRoles);
                if (resRoles?.data.data != null && resRoles.data.data.Count() > 0)
                    StaticData.Roles = resRoles.data.data;
                return "200";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void Role(Role? role, bool? createEdit,int? delete)
        {
            if (delete!=null)
            {
                var r = StaticData.Roles.SingleOrDefault(x => x.id == delete);
                StaticData.Roles.Remove(r);
            }
            else if (createEdit == false)
            {
                StaticData.Roles?.Add(role);
            }  
            else
            {
                Role? r = StaticData.Roles?.SingleOrDefault(x => x.id == role.id);
                if (r != null)
                {
                    r.selfRegister = role.selfRegister;
                    r.needsApproval = role.needsApproval;
                    r.claims = role.claims;
                    r.name = role.name;
                }
            }
        }
    //    public void GetAllControllersAndActions()
    //    {
    //        var controllersAndActions = new List<DTOs.Action>();

    //        // استخراج نوع تمامی کلاس‌ها در اسمبلی اصلی
    //        var assembly = Assembly.GetExecutingAssembly();

    //        // جستجو برای تمامی کلاس‌هایی که از ControllerBase ارث بری کرده‌اند
    //        var controllerTypes = assembly.GetTypes()
    //            .Where(t => t.IsSubclassOf(typeof(ControllerBase)) || t.IsSubclassOf(typeof(Controller)));

    //        foreach (var controller in controllerTypes)
    //        {
    //            // استخراج تمامی متدها (Actions) در هر کنترلر
    //            var actions = controller.GetMethods()
    //.Where(m => m.IsPublic &&
    //            !m.IsDefined(typeof(NonActionAttribute)) &&
    //            m.GetCustomAttributes<HttpMethodAttribute>(true).Any());

    //            foreach (var action in actions)
    //            {
    //                // پیدا کردن attributeهای مربوط به HTTP verb (مثل [HttpGet] یا [HttpPost])
    //                var httpVerbAttributes = action.GetCustomAttributes<HttpMethodAttribute>(true);

    //                if (!httpVerbAttributes.Any())
    //                {
    //                    controllersAndActions.Add(new DTOs.Action
    //                    {
    //                        Controller = controller.Name.Replace("Controller", ""),
    //                        Name = action.Name,
    //                        Verb = "GET"
    //                    });
    //                }
    //                else
    //                {
    //                    foreach (var verb in httpVerbAttributes)
    //                    {
    //                        controllersAndActions.Add(new DTOs.Action
    //                        {
    //                            Controller = controller.Name.Replace("Controller", ""),
    //                            Name = action.Name,
    //                            Verb = verb.HttpMethods.First().ToLower()
    //                        });
    //                    }
    //                }
    //            }
    //        }

    //        StaticData.Actions = controllersAndActions;
    //    }
        public void SetPublicKey(string publickey)
        {
            StaticData.PublicKey = publickey;
        }

        public async Task<string> GetAllTheClaims()
        {
            try
            {
                var responseRoles = await _client.GetAsync($"{_baseUrl}/api/v1/claim");
                DTO<Claim>? resRoles = null;
                var jsonStringRoles = await responseRoles.Content.ReadAsStringAsync();
                resRoles = JsonSerializer.Deserialize<DTO<Claim>>(jsonStringRoles);
                if (resRoles?.data.data != null && resRoles.data.data.Count() > 0)
                    StaticData.Claims = resRoles.data.data;
                return "200";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
