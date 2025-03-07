using identity.Core.Domain.Entities;
using identity.Infrastructure.Data;
using identity.Presentation.Models.BindingModel;
using identity.Presentation.Models.ResourceModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace identity.Interface.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ClaimController : ControllerBase
    {
        private readonly SilbargBaseIdentityContext _context;
        public ClaimController(SilbargBaseIdentityContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetClaims()
        {

            try
            {
                var res = await _context.Claims.AsNoTracking().Include(x => x.InverseParent).Where(x => x.ParentId == null).Select(x => new Claim1ResourceModel
                {
                    Id = x.Id,
                    FaName = x.FaName,
                    EnName = x.EnName,
                    Path= x.Path,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Verb = x.Verb,
                    IsCommon=x.IsCommon,
                    InverseParent = x.InverseParent.Select(m => new Claim1ResourceModel
                    {
                        Id = m.Id,
                        FaName = m.FaName,
                        EnName = m.EnName,
                        Path= m.Path,
                        IsActive = m.IsActive,
                        ParentId = m.ParentId,
                        Verb = m.Verb,
                        InverseParent = null,
                        IsCommon = m.IsCommon,
                    })
                }).ToListAsync();
                var year = await _context.Years.Where(x => x.Id != 0).MinAsync(x => x.Id);

                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        data = res,
                        message = year.ToString()
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClaimById(int id)
        {
            try
            {
                var res = await _context.Claims.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        data = res
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
        public async Task<IActionResult> CreateClaim([FromBody] CreateClaimBindingModel claim)
        {
            try
            {
                int id = await _context.Claims.AnyAsync() ? await _context.Claims.MaxAsync(z => z.Id) + 1 : 1;
                Claim item = new()
                {
                    Id = id,
                    FaName = claim.FaName,
                    EnName = claim.EnName,
                    Path=claim.Path,
                    IsActive = claim.IsActive,
                    ParentId = claim.ParentId == 0 ? null : claim.ParentId,
                    Verb = claim.Verb,

                };
                await _context.Claims.AddAsync(item);
                await _context.SaveChangesAsync();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        message = "آیتم با موفقیت ایجاد شد.",
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
        public async Task<IActionResult> UpdateClaim([FromBody] CreateClaimBindingModel claim)
        {
            try
            {
                var item = await _context.Claims.SingleOrDefaultAsync(x => x.Id == claim.Id);
                if (item != null)
                {
                    item.EnName = claim.EnName;
                    item.FaName = claim.FaName;
                    item.Path= claim.Path;
                    item.Verb = claim.Verb;
                    if (claim.ParentId != 0)
                        item.ParentId = claim.ParentId;
                    else item.ParentId = null;

                }
                await _context.SaveChangesAsync();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        message = "آیتم با موفقیت ویرایش شد."
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClaim(int id)
        {

            try
            {
                var item = await _context.Claims.SingleOrDefaultAsync(x => x.Id == id);
                if (item != null)
                {
                    _context.Claims.Remove(item);
                    await _context.SaveChangesAsync();
                    return Ok(new DTOResourceModel
                    {
                        status = 200,
                        message = "success",
                        data = new DataResourceModel
                        {
                            message = "دسترسی با موفقیت حذف شد."
                        },
                        errors = new List<string>(),
                        warning = new List<string>()
                    });
                }
                return NotFound(new DTOResourceModel
                {
                    status = 404,
                    message = "not found",
                    data = new DataResourceModel(),
                    errors = new List<string>() { "دسترسی پیدا نشد." },
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
    }
}
