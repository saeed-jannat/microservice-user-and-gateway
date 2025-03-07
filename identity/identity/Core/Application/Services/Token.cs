using identity.Core.Application.Interfaces;
using identity.Core.Domain.Entities;
using identity.Infrastructure.Data;
using identity.Presentation.Models.BindingModel;
using identity.Presentation.Models.ResourceModel;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace identity.Core.Application.Services
{
    public class Token : IToken
    {
        private readonly SilbargBaseIdentityContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _privatekey;

        public Token(SilbargBaseIdentityContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _privatekey = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure/Keys", "private_key.pem"));
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> BuildToken(string username, string password, int roleid, int organizationId, User user)
        {
            using var sha256 = SHA256.Create();
            //User user = await _context.Users.Include(x => x.UserRoles).SingleOrDefaultAsync(x => x.UserName == username && x.PassWord == password);
            var extra_claims = user.UserRoles.SingleOrDefault(x => x.RoleId == roleid)?.UserRoleClaims.Select(claim => new ClaimBindingModel
            {
                ClaimId = claim.ClaimsId,
                IsRevoked = claim.IsRevoked ?? false,
                Years = claim.Years.Select(x => x.Id).ToList()
            }).ToList();
            UserRole role = user.UserRoles.SingleOrDefault(x => x.OrganizationId == organizationId && x.RoleId == roleid);
            string sex = user.Sex == true ? "male" : "female";
            List<CoockieClaimResourceModel> add_claim = extra_claims.Where(x => x.IsRevoked == false).Select(x => new CoockieClaimResourceModel
            {
                Id = (int)x.ClaimId,
                IsRevoked = (bool)x.IsRevoked,
                Years = x.Years
            }).ToList();
            List<CoockieClaimResourceModel> remove_claim = extra_claims.Where(x => x.IsRevoked == true).Select(x => new CoockieClaimResourceModel
            {
                Id = (int)x.ClaimId,
                IsRevoked = (bool)x.IsRevoked,
                Years = x.Years
            }).ToList();
            List<int> zones = user.UserRoles.Select(x => (int)x.OrganizationId).ToList();
            if (user != null)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        WriteIndented = true
                    };

                    var rsa = RSA.Create();
                    rsa.ImportFromPem(_privatekey);

                    var securityKey = new RsaSecurityKey(rsa);
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

                    List<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>();
                    claims.Add(new System.Security.Claims.Claim(ClaimTypes.Expiration, DateTime.Now.AddDays(30).ToString()));
                    claims.Add(new System.Security.Claims.Claim("sex", sex));
                    claims.Add(new System.Security.Claims.Claim("userid", user.Id.ToString()));
                    claims.Add(new System.Security.Claims.Claim("role", role.Role.Id.ToString()));
                    claims.Add(new System.Security.Claims.Claim("add_claim", JsonSerializer.Serialize(add_claim, options)));
                    claims.Add(new System.Security.Claims.Claim("remove_claim", JsonSerializer.Serialize(remove_claim, options)));
                    claims.Add(new System.Security.Claims.Claim("years", JsonSerializer.Serialize(role.Years.Select(x => x.Id).ToList(), options)));
                    claims.Add(new System.Security.Claims.Claim("organization", JsonSerializer.Serialize(role.OrganizationId, options)));
                    var token = new JwtSecurityToken(
                        issuer: "https://taxapi.silbarg.ir",  // مشخص کردن تولید کننده توکن
                        audience: user.UserName, // مشخص کردن مخاطب توکن
                        claims: claims,
                        expires: DateTime.Now.AddDays(30), // زمان انقضای توکن
                        signingCredentials: credentials);

                    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                    return jwt;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Format Error: {ex.Message}");
                    return "unauthorized";
                }
                catch (CryptographicException ex)
                {
                    Console.WriteLine($"Cryptographic Error: {ex.Message}");
                    return "unauthorized";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unknown Error: {ex.Message}");
                    return "unauthorized";
                }

            }
            return "unauthorized";
        }
        public ClaimsPrincipal ValidationToken(string token)
        {
            var publicKey = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure/Keys", "public_key.pem"));
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
