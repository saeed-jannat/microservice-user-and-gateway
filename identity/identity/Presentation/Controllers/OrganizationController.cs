using identity.Core.Application.Interfaces;
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
    public class OrganizationController : ControllerBase
    {
        private readonly SilbargBaseIdentityContext _context;
        private readonly IDateService _date;
        public OrganizationController(SilbargBaseIdentityContext context, IDateService date)
        {
            _context = context;
            _date = date;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetOrganizations()
        {
            try
            {
                var zones = await _context.Organizations.AsNoTracking()
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.Address,
                        x.MemoryId,
                        x.NationalCode,
                        ZoneName = x.Zone.ZoneName,
                        CreatedDate = _date.ConvertToShamsi(x.CreatedDate),
                        UpdatedDate = _date.ConvertToShamsi(x.UpdatedDate)
                    }).ToListAsync();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        data = zones
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
        public async Task<IActionResult> GetSpecificZone(int id)
        {
            var res = await _context.Organizations.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            if (res != null)
            {
                try
                {
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
            else
            {
                return NotFound(new DTOResourceModel
                {
                    status = 404,
                    message = "not found",
                    data = new DataResourceModel
                    {
                        message = "آیتم پیدا نشد."
                    },
                    errors = new List<string>(),
                    warning = new List<string>()
                });
            }
        }
        [HttpPost("")]
        public async Task<IActionResult> CreateOrganization([FromBody] OrganizationBindingModel zone)
        {
            try
            {
                int id = await _context.Organizations.AnyAsync() ? await _context.Organizations.MaxAsync(z => z.Id) + 1 : 1;
                Organization item = new()
                {
                    Id = id,
                    Name = zone.Name,
                    Address = zone.Address,
                    NationalCode = zone.NationalCode,
                    MemoryId = zone.MemoryId,
                    CreatedDate = DateTime.Now,
                    ZoneId=zone.Id,
                };
                await _context.Organizations.AddAsync(item);
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
        public async Task<IActionResult> UpdateOrganization([FromBody] OrganizationBindingModel zone)
        {
            Organization item = await _context.Organizations.SingleOrDefaultAsync(x => x.Id == zone.Id);
            if (item != null)
            {
                try
                {
                    item.Address = zone.Address;
                    item.Name = zone.Name;
                    item.UpdatedDate = DateTime.Now;
                    item.NationalCode = zone.NationalCode;
                    item.MemoryId = zone.MemoryId;
                    await _context.SaveChangesAsync();
                    return Ok(new DTOResourceModel
                    {
                        status = 200,
                        message = "success",
                        data = new DataResourceModel()
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
            else
            {
                return NotFound(new DTOResourceModel
                {
                    status = 404,
                    message = "not found",
                    data = new DataResourceModel()
                    {
                        message = "آیتم پیدا نشد."
                    },
                    errors = new List<string>(),
                    warning = new List<string>()
                });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteZone(int id)
        {
            Organization item = await _context.Organizations.SingleOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                try
                {
                    _context.Organizations.Remove(item);
                    await _context.SaveChangesAsync();
                    return Ok(new DTOResourceModel
                    {
                        status = 200,
                        message = "success",
                        data = new DataResourceModel
                        {
                            message = "آیتم با موفقیت حذف شد."
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
            else
            {
                return NotFound(new DTOResourceModel
                {
                    status = 404,
                    message = "not found",
                    data = new DataResourceModel
                    {
                        message = "آیتم پیدا نشد."
                    },
                    errors = new List<string>(),
                    warning = new List<string>()
                });
            }
        }

    }
}
