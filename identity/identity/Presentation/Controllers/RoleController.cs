using identity.Core.Domain.Entities;
using identity.Infrastructure.Data;
using identity.Presentation.Models.BindingModel;
using identity.Presentation.Models.ResourceModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Claim = identity.Core.Domain.Entities.Claim;

namespace identity.Interface.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly SilbargBaseIdentityContext _context;
        public RoleController(SilbargBaseIdentityContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _context.Roles.AsNoTracking().Select(x => new { x.Id, x.Name, x.Claims, x.NeedsApproval, x.SelfRegister }).ToListAsync();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        data = roles
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
                    errors = new List<string>() { "خطا در سرور با پشتیبانی تماس بگیرید." },
                    warning = new List<string>()
                });
            }

        }
        [HttpGet("{id}")]
        public IActionResult GetRoleById(int id)
        {
            try
            {
                var role = _context.Roles.AsNoTracking().SingleOrDefault(x => x.Id == id);
                if (role == null)
                {
                    return NotFound(new DTOResourceModel
                    {
                        status = 404,
                        message = "not found",
                        data = new DataResourceModel(),
                        errors = new List<string>() { "آیتم پیدا نشد." },
                        warning = new List<string>()
                    });
                }
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        data = role
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
        [HttpGet("claims/{id}")]
        public async Task<IActionResult> RoleClaims(int id)
        {
            try
            {
                var role = await _context.Roles.AsNoTracking().Include(x => x.Claims).SingleOrDefaultAsync(x => x.Id == id);
                if (role == null)
                {
                    return NotFound(new DTOResourceModel
                    {
                        status = 404,
                        message = "not found",
                        data = new DataResourceModel(),
                        errors = new List<string>() { "آیتم پیدا نشد." },
                        warning = new List<string>()
                    });
                }

                var Claims = role.Claims.Select(x => new { x.Id, x.FaName, x.EnName }).ToList();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        data = Claims
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
        [HttpPost("")]
        public async Task<IActionResult> CreateRole([FromBody] RoleBindingModel role)
        {
            try
            {
                int id = await _context.Roles.AnyAsync() ? await _context.Roles.MaxAsync(z => z.Id) + 1 : 1;
                Role r = new()
                {
                    Id = id,
                    Name = role.Name,
                    NeedsApproval = role.NeedsApproval,
                    SelfRegister = role.SelfRegister,
                };
                if (role.Claims != null)
                {
                    foreach (var c in role.Claims)
                    {
                        Claim claim = await _context.Claims.SingleOrDefaultAsync(x => x.Id == c.Id);
                        if (claim != null)
                            r.Claims.Add(claim);
                    }
                }
                await _context.Roles.AddAsync(r);
                await _context.SaveChangesAsync();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel()
                    {
                        message = "نقش جدید با موفقیت اضافه شد.",
                        data = new List<int>() { id }
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
        [HttpPut("")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleBindingModel role)
        {
            try
            {
                Role r = await _context.Roles.SingleOrDefaultAsync(x => x.Id == role.Id);
                if (r != null)
                {
                    r.NeedsApproval = role.NeedsApproval;
                    r.SelfRegister = role.SelfRegister;
                    r.Name = role.Name;
                }
                var R = await _context.Roles
                    .Include(r => r.Claims)
                    .SingleOrDefaultAsync(x => x.Id == role.Id);

                if (R != null && R.Claims != null)
                {
                    R.Claims.Clear();
                    foreach (var c in R.Claims.ToList()) // تبدیل به لیست برای اجتناب از خطای تغییر مجموعه در حین پیمایش
                    {
                        R.Claims.Remove(c);
                    }
                }
                if (role.Claims != null)
                {
                    foreach (var c in role.Claims)
                    {
                        Claim claim = await _context.Claims.SingleOrDefaultAsync(x => x.Id == c.Id);
                        if (claim != null)
                            r.Claims.Add(claim);
                    }
                }

                //await _context.Roles.AddAsync(r);
                await _context.SaveChangesAsync();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel()
                    {
                        message = "نقش با موفقیت ویرایش شد"
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
                    errors = new List<string>() { "خطایی در سرور رخ داد با پشتیبانی تماس بگیرید" },
                    warning = new List<string>()
                });
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                Role r = await _context.Roles.Include(x => x.Claims).SingleOrDefaultAsync(x => x.Id == id);
                if (r != null)
                {
                    r.Claims.Clear();
                    //_context.RoleCl
                    _context.Roles.Remove(r);
                    await _context.SaveChangesAsync();
                    return Ok(new DTOResourceModel
                    {
                        status = 200,
                        message = "success",
                        data = new DataResourceModel()
                        {
                            message = "نقش با موفقیت حذف شد."
                        },
                        errors = new List<string>(),
                        warning = new List<string>()
                    });
                }
                else
                {
                    return NotFound(new DTOResourceModel
                    {
                        status = 404,
                        message = "not found",
                        data = new DataResourceModel(),
                        errors = new List<string>() { "نقش پیدا نشد." },
                        warning = new List<string>()
                    });
                }
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
    }
}
