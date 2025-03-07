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
    public class ZoneController : ControllerBase
    {
        private readonly SilbargBaseIdentityContext _context;
        public ZoneController(SilbargBaseIdentityContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public async Task<IActionResult> Zones()
        {
            try
            {
                var states = await _context.Zones.Where(x => x.HczoneLevelCode==6&& x.ParentId == null).Select(x => new { x.ZoneId, x.ZoneName }).ToListAsync();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        message = "لیست استان ها.",
                        data = states
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
        public async Task<IActionResult> Zone(int id)
        {
            try
            {
                var Cities = await _context.Zones.Where(x => x.ParentId == id).Select(x => new { x.ZoneId, x.ZoneName }).ToListAsync();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        message = "لیست شهر ها.",
                        data = Cities
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
        public async Task<IActionResult> create([FromBody] CountryDivisionBindingModel data)
        {
            try
            {
                long id = await _context.Zones.AnyAsync() ? await _context.Zones.MaxAsync(z => z.ZoneId) + 1 : 1;
                Zone item = new()
                {
                    ZoneId = id,
                    ZoneName = data.Name,
                    ParentId = data.ParentId,
                };
                await _context.Zones.AddAsync(item);
                await _context.SaveChangesAsync();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        data = new List<long>() { id }
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
        public async Task<IActionResult> Edit([FromBody] CountryDivisionBindingModel data)
        {
            try
            {
                Zone item = await _context.Zones.SingleOrDefaultAsync(x => x.ZoneId == data.Id);
                if (data.ParentId != null && data.ParentId != 0)
                    item.ParentId = data.ParentId;
                item.ZoneName = data.Name;
                await _context.SaveChangesAsync();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        message = $"{item.HczoneLevelName} با موفقیت ویرایش شد"
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Zone item = await _context.Zones.SingleOrDefaultAsync(x => x.ZoneId == id);
                if (item != null)
                {
                    _context.Zones.Remove(item);
                    await _context.SaveChangesAsync();
                    return Ok(new DTOResourceModel
                    {
                        status = 200,
                        message = "success",
                        data = new DataResourceModel
                        {
                            message = $"{item.HczoneLevelName} با موفقیت حذف شد"
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
                        data = new DataResourceModel
                        {
                            message = "آیتم پیدا نشد."
                        },
                        errors = new List<string>(),
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
