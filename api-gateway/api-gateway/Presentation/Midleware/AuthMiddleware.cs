using api_gateway.Core.Application.DTOs;
using api_gateway.Core.Application.Interfaces;
using api_gateway.Core.Common.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace api_gateway.Presentation.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider IServiceProvider)
        {
            string path = context.Request.Path.Value.ToLower();
            //string regexPattern = ConvertPathToRegex(path);
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                await RedirectToLogin(context);
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var scope = IServiceProvider.CreateScope();
            IEnumerable<ClaimsIdentity> identities = null;
            ICookie tokenService = null;
            try
            {
                tokenService = scope.ServiceProvider.GetRequiredService<ICookie>();
                identities = tokenService.ValidationToken(token)?.Identities;
            }
            finally
            {
                scope.Dispose();
            }
            if (identities == null || !identities.Any())
            {
                await RedirectToLogin(context);
                return;
            }

            var claims = identities.First().Claims;
            var claimsDict = claims.ToDictionary(c => c.Type.Split('/').Last(), c => c.Value);

            if (!claimsDict.TryGetValue("role", out var roleId))
            {
                await RedirectToUnauthorized(context);
                return;
            }

            //var role = StaticData.Roles?.SingleOrDefault(x => x.id == Convert.ToInt32(roleId));
            //List<Core.Application.DTOs.Claim> roleClaims = role?.claims ?? new List<Core.Application.DTOs.Claim>();

            var httpMethod = context.Request.Method.ToLower();

            //var authorizedClaim = roleClaims.SingleOrDefault(c =>
            //        c.verb.ToLower() == httpMethod &&
            //        Regex.IsMatch(path, Authorization.ConvertPathToRegex(c.path.ToLower()), RegexOptions.IgnoreCase)
            //    );
            //|| roleId == "2"
            //Claim currentClaim = null;
            //try { 
            //    currentClaim = StaticData.Claims?.SelectMany(model => new[] { model }.Concat(model.inverseParent)).SingleOrDefault(c =>
            //        c.verb.ToLower() == httpMethod &&
            //        Regex.IsMatch(path, Authorization.ConvertPathToRegex(c.path.ToLower()), RegexOptions.IgnoreCase)
            //    ); ;
            //}
            //finally
            //{
            if (Authorization.IsAuthorized(token, context.Request.Path.Value.ToLower(), httpMethod))
                {
                    if (claimsDict.TryGetValue("userid", out var userId) && !string.IsNullOrEmpty(userId))
                    {
                        context.Request.Headers["X-User-Id"] = userId;
                    }
                    if (claimsDict.TryGetValue("role", out var roleValue) && !string.IsNullOrEmpty(roleValue))
                    {
                        context.Request.Headers["X-User-Role"] = roleValue;
                    }
                    if (claimsDict.TryGetValue("zone", out var zone) && !string.IsNullOrEmpty(zone))
                    {
                        context.Request.Headers["X-User-Zone"] = string.Join(",", zone);
                    }
                    await _next(context);
                }
                else
                {
                    await RedirectToUnauthorized(context);
                }
            //}
        }



        private static async Task RedirectToLogin(HttpContext context)
        {
            var responseObj = new
            {
                status = 302,
                message = "redirect",
                errors = new[] { "" }
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status302Found;
            context.Response.Headers["Location"] = "http://localhost:5012/auth/login"; // آدرس مقصد
            var jsonResponse = JsonSerializer.Serialize(responseObj);
            await context.Response.WriteAsync(jsonResponse);
        }

        private static async Task RedirectToUnauthorized(HttpContext context)
        {
            var responseObj = new
            {
                status = 401,
                message = "unauthorized",
                errors = new[] { "شما به این بخش دسترسی ندارید" }
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            var jsonResponse = JsonSerializer.Serialize(responseObj);
            await context.Response.WriteAsync(jsonResponse);
        }


    }

    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
