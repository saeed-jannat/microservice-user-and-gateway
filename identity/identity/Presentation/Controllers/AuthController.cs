using identity.Core.Application.Interfaces;
using identity.Core.Domain.Entities;
using identity.Infrastructure.Data;
using identity.Presentation.Models.BindingModel;
using identity.Presentation.Models.ResourceModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace identity.Interface.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SilbargBaseIdentityContext _context;
        private readonly IToken _token;
        public AuthController(SilbargBaseIdentityContext context, IToken token)
        {
            _context = context;
            _token = token;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserBindingModel data)
        {
            var public_role = await _context.Roles.AsNoTracking().Where(x => x.SelfRegister == true).Select(x => x.Id).ToListAsync();
            var x = await _context.Roles.AsNoTracking().Where(r => r.RoleParentChildParentRoles.Count() == 0).ToListAsync();
            if (public_role.Contains((int)data.RoleId))
            {
                try
                {
                    List<string> errors = new List<string>();
                    if (await _context.Users.AnyAsync(x => x.UserName == data.UserName))
                        errors.Add("نام کاربری تکراری است.");
                    if (await _context.Users.AnyAsync(x => x.PhoneNumber == data.PhoneNumber))
                        errors.Add("شماره تلفن برای شخص دیگری ثبت شده است.");
                    if (await _context.Users.AnyAsync(x => x.NationalCode == data.NationalCode))
                        errors.Add("شماره ملی وارد شده برای شخص دیگری ثبت شده است.");
                    if (await _context.Organizations.SingleOrDefaultAsync(x => x.Id == data.ZoneId) == null)
                        errors.Add("شرکت انتخابی یافت نشد با پشتیبانی تماس بگیرید.");
                    if (errors.Count() > 0)
                    {
                        return BadRequest(
                            new DTOResourceModel
                            {
                                status = 400,
                                message = "bad request",
                                data = new DataResourceModel(),
                                errors = errors,
                                warning = new List<string>()
                            }
                            );
                    }
                    int uid = await _context.Users.AnyAsync() ? await _context.Users.MaxAsync(z => z.Id) + 1 : 1;
                    var sha256 = SHA256.Create();
                    var hashedPass = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(data.PassWord)));

                    User u = new()
                    {
                        Id = uid,
                        UserName = data.UserName,
                        PassWord = hashedPass,
                        Sex = (bool)data.Sex,
                        FirstName = data.FirstName,
                        LastName = data.LastName,
                        CreatedDate = DateTime.Now,
                        Description = data.Description,
                        NationalCode = data.NationalCode,
                        PhoneNumber = data.PhoneNumber,
                        Status = true,
                        SubscriptionEndDate = DateTime.Now.AddYears(1),
                    };
                    await _context.Users.AddAsync(u);
                    await _context.SaveChangesAsync();
                    int urid = await _context.UserRoles.AnyAsync() ? await _context.UserRoles.MaxAsync(z => z.Id) + 1 : 1;
                    UserRole ur = new()
                    {
                        Id = urid,
                        RoleId = data.RoleId,
                        StartDate = DateTime.Now,
                        UserId = uid,
                        OrganizationId = data.OrganizationId,
                    };
                    await _context.UserRoles.AddAsync(ur);
                    await _context.SaveChangesAsync();
                    string token = Convert.ToString(_token.BuildToken(u.UserName, u.PassWord, (int)data.RoleId, (int)data.ZoneId, u).Result);
                    return Ok(new DTOResourceModel
                    {
                        status = 200,
                        message = "success",
                        data = new DataResourceModel
                        {
                            data = token,
                            message = "ثبت نام شما با موفقیت انجام شد."
                        },
                        errors = new List<string>(),
                        warning = new List<string>()
                    });
                }
                catch 
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new DTOResourceModel
                    {
                        status = 500,
                        message = "fail",
                        data = new DataResourceModel(),
                        errors = new List<string>() { "خطایی در سرور رخ داد با پشتیبانی تماس بگیرید." },
                        warning = new List<string>()
                    });
                }

            }
            return BadRequest(
                new DTOResourceModel
                {
                    status = 400,
                    message = "bad request",
                    data = new DataResourceModel(),
                    errors = new List<string>() { "شما مجاز به ثبت نام با نقش انتخابی نیستید." },
                    warning = new List<string>()
                }
                );
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginBindingModel data)
        {
            using var sha256 = SHA256.Create();
            string pass = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(data.PassWord)));

            var user = await _context.Users.AsNoTracking()
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .Include(x => x.UserRoles)
                    .ThenInclude(urc => urc.UserRoleClaims)
                        .ThenInclude(urc => urc.Years)
                .Include(y => y.UserRoles)
                    .ThenInclude(y => y.Years)
                .SingleOrDefaultAsync(x => x.UserName == data.UserName && x.PassWord == pass);
            if (user != null)
            {
                try
                {
                    string token = Convert.ToString(_token.BuildToken(user.UserName, pass, data.RoleId, data.OrganizationId, user).Result);
                    if (token != "unauthorized")
                    {
                        return Ok(new DTOResourceModel
                        {
                            status = 200,
                            message = "success",
                            data = new DataResourceModel()
                            {
                                message = $".{user.FirstName} عزیز با نقش {user.UserRoles.SingleOrDefault(x => x.RoleId == data.RoleId && x.OrganizationId == data.OrganizationId)?.Role?.Name} وارد شدید",
                                data = token.ToString() ,
                            },
                            errors = new List<string>(),
                            warning = new List<string>()
                        });
                    }
                    return Unauthorized(new DTOResourceModel
                    {
                        status = 401,
                        message = "unauthorized",
                        data = new DataResourceModel(),
                        errors = new List<string>() { "ابتدا ثبت نام کنید" },
                        warning = new List<string>()
                    }
                    );

                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new DTOResourceModel
                    {
                        status = 500,
                        message = "fail",
                        data = new DataResourceModel(),
                        errors = new List<string>() { "خطایی در سرور رخ داد با پشتیبانی تماس بگیرید." },
                        warning = new List<string>()
                    });
                }

            }
            return BadRequest(
                new DTOResourceModel
                {
                    status = 400,
                    message = "bad request",
                    data = new DataResourceModel(),
                    errors = new List<string>() { "رمز عبور یا نام کاربری صحیح نیست." },
                    warning = new List<string>()
                }
            );

        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            try
            {
                int roleid = (int)Convert.ToUInt32(HttpContext.Request.Headers["X-User-Role"].ToString());
                int userid = (int)Convert.ToUInt32(HttpContext.Request.Headers["X-User-Id"].ToString());
                User? user = await _context.Users.Include(x => x.UserRoles)
                    .ThenInclude(r=>r.Role)
                    .SingleOrDefaultAsync(x => x.Id == userid);
                string? role = user?.UserRoles.FirstOrDefault(x => x.RoleId == roleid)?.Role?.Name;
                CreateUserBindingModel u = new()
                {
                    ZoneId=user?.ZoneId,
                    Description=user?.Description,
                    FirstName=user?.FirstName,
                    Id=user?.Id,
                    LastName=user?.LastName,
                    NationalCode= user?.NationalCode,
                    PhoneNumber=user?.PhoneNumber,
                    Sex=user?.Sex,
                    UserName= user?.UserName,
                    RoleName= role
                };
                if (user == null)
                {
                    return NotFound(new DTOResourceModel
                    {
                        status = 404,
                        message = "not found",
                        data = new DataResourceModel(),
                        errors = new List<string>() { "کاربر یافت نشد." },
                        warning = new List<string>()
                    });
                }
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel()
                    {
                        message = $".{user?.FirstName} عزیز با نقش {role} وارد شدید",
                        data = new List<CreateUserBindingModel>() { u },
                    },
                    errors = new List<string>(),
                    warning = new List<string>()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new DTOResourceModel
                {
                    status = 500,
                    message = "fail",
                    data = new DataResourceModel(),
                    errors = new List<string>() {  },
                    warning = new List<string>()
                });
            }
        }
    }
}
