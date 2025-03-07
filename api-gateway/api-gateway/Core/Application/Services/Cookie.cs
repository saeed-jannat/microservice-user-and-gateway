using api_gateway.Core.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace api_gateway.Core.Application.Services
{
    public class Cookie : ICookie
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Cookie(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public ClaimsPrincipal? ValidationToken(string token)
        {
            var publicKey = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure", "Keys", "public_key.pem"));
            //var publicKey = File.ReadAllText("/run/secrets/public_key");
            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);

            var securityKey = new RsaSecurityKey(rsa);
            var tokenHandler = new JwtSecurityTokenHandler();
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,   // بررسی تاریخ انقضا
                ValidIssuer = "https://taxapi.silbarg.ir",  // تولیدکننده توکن
                ValidAudience = null,      // اگر نمی‌خواهید بررسی کنید می‌توانید حذف کنید
                IssuerSigningKey = securityKey,   // کلید عمومی برای امضای توکن
                ClockSkew = TimeSpan.Zero // انحرافات زمانی خیلی جزئی نادیده گرفته می‌شود
            };

            try
            {
                // Validate the token
                var principal = tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);

                // بررسی برخی از claimsهای خاص که شما در توکن قرار داده‌اید
                var userid = principal.FindFirst("userid")?.Value;
                var role = principal.FindFirst("role")?.Value;
                var sex = principal.FindFirst("sex")?.Value;

                Console.WriteLine($"UserID: {userid}, Role: {role}, Sex: {sex}");

                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                Console.WriteLine($"Token has expired: {ex.Message}");
                return null;
            }
            catch (SecurityTokenValidationException ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation error: {ex.Message}");
                return null;
            }
        }
    }
}
