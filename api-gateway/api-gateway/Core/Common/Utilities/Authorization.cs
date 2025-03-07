using api_gateway.Core.Application.DTOs;
using api_gateway.Core.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace api_gateway.Core.Common.Utilities
{
    public static class Authorization
    {
        public static IServiceProvider Instance { get; private set; }
        public static void Initialize(IServiceProvider serviceProvider)
        {
            Instance = serviceProvider;
        }
        public static bool IsAuthorized(string token,string path, string verb)
        {
            //using var scope = serviceProvider.CreateScope();
            
            Claim? currentClaim = StaticData.Claims?.SelectMany(model => new[] { model }.Concat(model.inverseParent)).SingleOrDefault(c =>
                    c.verb.ToLower() == verb &&
                    Regex.IsMatch(path, ConvertPathToRegex(c.path.ToLower()), RegexOptions.IgnoreCase)
                ); 
            if (currentClaim?.isCommon == true)
                return true;

            //var tokenService = Instance.GetService<ICookie>();
            using var scope = Instance.CreateScope();
            var tokenService = scope.ServiceProvider.GetRequiredService<ICookie>();
            var identities = tokenService.ValidationToken(token)?.Identities;
            var claims = identities.First().Claims;
            var claimsDict = claims.ToDictionary(c => c.Type.Split('/').Last(), c => c.Value);
            var roleId=claimsDict["role"];
            var role = StaticData.Roles?.SingleOrDefault(x => x.id == Convert.ToInt32(roleId));
            List<Claim> roleClaims = role?.claims ?? new List<Claim>();
            var authorizedClaim = roleClaims.SingleOrDefault(c =>
                    c.verb.ToLower() == verb &&
                    Regex.IsMatch(path, Authorization.ConvertPathToRegex(c.path.ToLower()), RegexOptions.IgnoreCase)
                );
            List<CoockieClaim> removeClaims = new();
            List<CoockieClaim> addClaims = new();
            string regexPattern;
            if (authorizedClaim != null)
            {
                regexPattern = ConvertPathToRegex(authorizedClaim?.path.ToLower());
                if (claimsDict.TryGetValue("remove_claim", out var removeClaimJson))
                {
                    removeClaims = JsonSerializer.Deserialize<List<CoockieClaim>>(removeClaimJson) ?? new List<CoockieClaim>();
                }
                if (claimsDict.TryGetValue("add_claim", out var addClaimJson))
                {
                    addClaims = JsonSerializer.Deserialize<List<CoockieClaim>>(addClaimJson) ?? new List<CoockieClaim>();
                }

                if (authorizedClaim != null && !removeClaims.Any(x => x.Id == authorizedClaim.id))
                    return true;

                var extraClaim = StaticData.Claims?.SingleOrDefault(x =>
                    addClaims.Any(c => c.Id == x.id) &&
                    x.path.ToLower() == authorizedClaim.path.ToLower() &&
                    x.verb.ToLower() == verb
                    );

                return extraClaim != null;
            }
            else
            {
                return false;
            }

        }
        public static string ConvertPathToRegex(string pathPattern)
        {
            //return "^" + Regex.Escape(path)
            //    .Replace("\\{id\\}", "[^/]+")  // `{id}` باید یک مقدار متغیر باشه (مثلاً عدد یا متن)
            //    .Replace("\\*", ".*")          // `*` یعنی هر مقدار
            //    + "$";
            pathPattern = Regex.Replace(pathPattern, "{id}", @"\d+");

            // جایگزینی * با هر مقدار دلخواه (.*)
            pathPattern = Regex.Replace(pathPattern, @"\*", @".*");

            // اضافه کردن ^ و $ برای بررسی کامل مسیر
            return $"^{pathPattern}$";
        }
    }
}
