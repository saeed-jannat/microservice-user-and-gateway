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
    public class UserController : ControllerBase
    {
        private readonly SilbargBaseIdentityContext _context;
        private readonly IDateService _date;
        public UserController(SilbargBaseIdentityContext context, IDateService date)
        {
            _context = context;
            _date = date;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetUsers()
        {

            try
            {
                var res = await _context.Users.AsNoTracking()
                    .Select(x => new
                    {
                        x.Id,
                        x.FirstName,
                        x.LastName,
                        x.NationalCode,
                        x.PhoneNumber,
                        SubscriptionEndDate = _date.ConvertToShamsi(x.SubscriptionEndDate),
                        x.Sex,
                        x.Status,
                        x.Description,
                        CreatedDate = _date.ConvertToShamsi(x.CreatedDate),
                        UpdatedDate = x.UpdatedDate != null ? _date.ConvertToShamsi(x.UpdatedDate) : "",
                        ZoneName = x.Zone.ZoneName,
                    }).ToListAsync();
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var res = await _context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
                if (res == null)
                {
                    return NotFound(new DTOResourceModel
                    {
                        status = 404,
                        message = "not found",
                        data = new DataResourceModel
                        {
                            message = "کاربر پیدا نشد"
                        },
                        errors = new List<string>(),
                        warning = new List<string>()
                    });
                }
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
        public async Task<IActionResult> CreateUser([FromBody] CreateUserBindingModel data)
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
                //if (await _context.Zones.SingleOrDefaultAsync(x => x.Id == data.ZoneId) == null)
                //    errors.Add("شرکت انتخابی یافت نشد با پشتیبانی تماس بگیرید.");
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
                using var sha256 = SHA256.Create();
                int id = await _context.Users.AnyAsync() ? await _context.Users.AsNoTracking().MaxAsync(z => z.Id) + 1 : 1;
                string pass = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(data.PassWord)));
                User item = new()
                {
                    Id = id,
                    UserName = data.UserName,
                    PassWord = pass,
                    Sex = (bool)data.Sex,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    CreatedDate = DateTime.Now,
                    Description = data.Description,
                    NationalCode = data.NationalCode,
                    PhoneNumber = data.PhoneNumber,
                    Status = true,
                    ZoneId = data.ZoneId
                };
                if (data.SubscriptionEndDate != null)
                {
                    item.SubscriptionEndDate = DateTime.Parse(data.SubscriptionEndDate);
                }
                await _context.Users.AddAsync(item);
                await _context.SaveChangesAsync();
                int userRoleId = 0;
                if (data.UserRoles != null)
                {
                    if (data.UserRoles.Count() > 0)
                    {
                        userRoleId = await _context.UserRoles.AnyAsync() ? await _context.UserRoles.AsNoTracking().MaxAsync(z => z.Id) + 1 : 1;
                        foreach (var r in data.UserRoles)
                        {
                            UserRole userrole = new()
                            {
                                Id = userRoleId,
                                UserId = id,
                                OrganizationId = r.OrganizationId,
                                StartDate = DateTime.Now,
                                RoleId = r.RoleId,
                            };
                            await _context.UserRoles.AddAsync(userrole);
                            try
                            {
                                await _context.SaveChangesAsync();
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

                            int userRoleClaimId = await _context.UserRoleClaims.AnyAsync() ? await _context.UserRoleClaims.AsNoTracking().MaxAsync(z => z.Id) + 1 : 1;
                            foreach (var urc in r.Claims)
                            {
                                if (urc.Id != 0)
                                {
                                    UserRoleClaim userRoleClaim = new()
                                    {
                                        Id = userRoleClaimId,
                                        UserRoleId = userRoleId,
                                        IsRevoked = urc.IsRevoked,
                                        ClaimsId = urc.Id,
                                    };
                                    await _context.UserRoleClaims.AddAsync(userRoleClaim);
                                    userRoleClaimId++;
                                }
                            }

                            try
                            {
                                await _context.SaveChangesAsync();
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
                            userRoleId++;
                        }

                    }
                }

                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        data = new List<int>() { id },
                        message = "کاربر با موفقیت ایجاد شد."
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
        public async Task<IActionResult> UpdateUser([FromBody] CreateUserBindingModel data)
        {
            User item = await _context.Users.SingleOrDefaultAsync(x => x.Id == data.Id);
            if (item != null)
            {
                try
                {
                    using var sha256 = SHA256.Create();
                    if (data.PassWord != null)
                        item.PassWord = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(data.PassWord)));
                    if (data.UserName != null)
                        item.UserName = data.UserName;
                    if (data.FirstName != null)
                        item.FirstName = data.FirstName;
                    if (data.LastName != null)
                        item.LastName = data.LastName;
                    if (data.Status != null)
                        item.Status = data.Status;
                    if (data.Description != null)
                        item.Description = data.Description;
                    if (data.SubscriptionEndDate != null)
                    {
                        item.SubscriptionEndDate = DateTime.Parse(data.SubscriptionEndDate);
                    }
                    if (data.Sex != null)
                    {
                        item.Sex = (bool)data.Sex;
                    }
                    if (data.PhoneNumber != null)
                    {
                        item.PhoneNumber = data.PhoneNumber;
                    }
                    if (data.ZoneId != 0)
                        item.ZoneId = data.ZoneId;
                    item.UpdatedDate = DateTime.Now;

                    int userRoleClaimId = await _context.UserRoleClaims.AnyAsync() ? await _context.UserRoleClaims.AsNoTracking().MaxAsync(z => z.Id) + 1 : 1;
                    int userRoleId = await _context.UserRoles.AnyAsync() ? await _context.UserRoles.AsNoTracking().MaxAsync(z => z.Id) + 1 : 1;
                    if (data.UserRoles != null)
                    {
                        foreach (var ur in data.UserRoles)
                        {
                            if (ur.IsRevoked == true)
                            {
                                var resURDelete = (ObjectResult)await DeleteUserRole(false, (int)ur.Id, ur.Years);
                                if (resURDelete.StatusCode != 200)
                                {
                                    return resURDelete;
                                }
                            }
                            else if (ur.IsRevoked == false)
                            {
                                var urYears = await _context.Years.Where(x => ur.Years.Contains(x.Id)).ToListAsync();
                                var userrole = await _context.UserRoles
                                    .SingleOrDefaultAsync(x => x.UserId == ur.UserId && x.OrganizationId == ur.OrganizationId && x.RoleId == ur.RoleId);
                                if (userrole == null)
                                {
                                    UserRole nur = new()
                                    {
                                        Id = userRoleId,
                                        StartDate = DateTime.Now,
                                        RoleId = ur.RoleId,
                                        UserId = ur.UserId,
                                        OrganizationId = ur.OrganizationId,
                                        Years = urYears
                                    };
                                    await _context.UserRoles.AddAsync(nur);
                                }
                                else
                                {
                                    foreach (var year in urYears)
                                    {
                                        userrole.Years.Add(year);
                                    }
                                }
                                if (ur.Claims != null)
                                {
                                    foreach (var urc in ur.Claims)
                                    {
                                        var urcYears = await _context.Years.Where(x => urc.Years.Contains(x.Id)).ToListAsync();
                                        UserRoleClaim nurc = new()
                                        {
                                            Id = userRoleClaimId,
                                            ClaimsId = urc.Id,
                                            IsRevoked = urc.IsRevoked,
                                            Years = urcYears,
                                            UserRoleId = userRoleId,
                                        };
                                        await _context.UserRoleClaims.AddAsync(nurc);
                                        userRoleClaimId++;
                                    }
                                }
                                userRoleId++;
                            }
                            else if (ur.IsRevoked == null)
                            {
                                if (ur.Claims != null)
                                {
                                    foreach (var urc in ur.Claims)
                                    {
                                        if (urc.Delete == true)
                                        {
                                            var resURCDelete = (ObjectResult)await DeleteUserClaim((int)urc.ClaimId, (int)ur.Id, (bool)urc.IsRevoked);
                                            if (resURCDelete.StatusCode != 200)
                                            {
                                                return resURCDelete;
                                            }
                                        }
                                        else
                                        {
                                            var urcYears = await _context.Years.Where(x => x.Id == urc.Year).ToListAsync();
                                            var condition = await _context.UserRoleClaims.SingleOrDefaultAsync(x => x.UserRoleId == ur.Id && x.ClaimsId == urc.ClaimId && x.IsRevoked == urc.IsRevoked);
                                            if (condition == null)
                                            {
                                                UserRoleClaim nurc = new()
                                                {
                                                    Id = userRoleClaimId,
                                                    UserRoleId = (int)ur.Id,
                                                    ClaimsId = urc.ClaimId,
                                                    IsRevoked = urc.IsRevoked,
                                                    Years = urcYears,
                                                };
                                                await _context.UserRoleClaims.AddAsync(nurc);
                                                userRoleClaimId++;
                                            }

                                        }

                                    }
                                }
                            }
                        }
                    }
                    // Remove UserRoleClaims that are no longer needed

                    await _context.SaveChangesAsync();

                    return Ok(new DTOResourceModel
                    {
                        status = 200,
                        message = "success",
                        data = new DataResourceModel
                        {
                            message = "اطلاعات کاربر با موفقیت ویرایش شد.",
                            data = new List<string>()
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
                        data = new DataResourceModel() { data = new List<string>() },
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
                    data = new DataResourceModel { message = "کاربر پیدا نشد." },
                    errors = new List<string>(),
                    warning = new List<string>()
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.Include(x => x.UserRoles).SingleOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {
                    return NotFound(new DTOResourceModel
                    {
                        status = 404,
                        message = "not found",
                        data = new DataResourceModel
                        {
                            message = "کاربر پیدا نشد"
                        },
                        errors = new List<string>(),
                        warning = new List<string>()
                    });
                }
                var user_parent_child = await _context.UserParentChildren.Where(x => x.ParentUserId == id || x.ChildUserId == id).ToListAsync();

                var user_role_claims = await _context.UserRoleClaims.Where(x => x.UserRole.UserId == id).ToListAsync();
                if (user_role_claims.Any())
                    _context.UserRoleClaims.RemoveRange(user_role_claims);
                if (user.UserRoles.Any())
                    _context.UserRoles.RemoveRange(user.UserRoles);
                if (user_parent_child.Any())
                    _context.UserParentChildren.RemoveRange(user_parent_child);
                if (user != null)
                {
                    _context.Users.Remove(user);
                }
                await _context.SaveChangesAsync();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        message = "کاربر با موفقیت حذف شد."
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
        #region user role
        [HttpGet("roles")]
        public async Task<IActionResult> UserRoles(int? userid, string? username, string? pass)
        {
            try
            {
                using var sha256 = SHA256.Create();
                string hashedpass = null;
                if (pass != null)
                    hashedpass = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(pass)));
                List<UserRoleBindingModel> userrole = await _context.UserRoles.Include(x => x.UserRoleClaims).Include(x => x.Years).Include(x => x.Role).Where(x =>
                x.UserId == userid &&
                x.EndDate == null ||
                x.User.UserName == username &&
                x.User.PassWord == hashedpass &&
                x.EndDate == null
                )
                    .Select(x => new UserRoleBindingModel
                    {
                        Name = x.Role.Name,
                        Id = x.Id,
                        OrganizationId = x.OrganizationId,
                        UserId = x.UserId,
                        RoleId = x.RoleId,
                        StartDate = x.StartDate,
                        Years = (List<int>)x.Years.Select(y => y.Id),
                        Claims = (List<ClaimBindingModel>)x.UserRoleClaims.Select(y => new ClaimBindingModel
                        {
                            ClaimId = y.ClaimsId,
                            Id = y.Id,
                            IsRevoked = y.IsRevoked,
                            Years = (List<int>)y.Years.Select(x => x.Id)
                        })
                    }).ToListAsync();
                if (userrole.Any())
                {
                    return Ok(new DTOResourceModel
                    {
                        status = 200,
                        message = "success",
                        data = new DataResourceModel()
                        {
                            message = $"نقش ها و دسترسی های کاربران",
                            data = userrole
                        },
                        errors = new List<string>(),
                        warning = new List<string>()
                    });
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
        [HttpPost("role")]
        public async Task<IActionResult> UserRole([FromBody] UserRoleBindingModel data)
        {
            try
            {
                int id = await _context.UserRoles.AnyAsync() ? await _context.UserRoles.MaxAsync(z => z.Id) + 1 : 1;
                UserRole ur = new()
                {
                    Id = id,
                    RoleId = data.RoleId,
                    StartDate = DateTime.Now,
                    UserId = data.UserId,
                    OrganizationId = data.OrganizationId,
                };
                await _context.UserRoles.AddAsync(ur);
                foreach (var item in data.Claims)
                {
                    int urcid = await _context.UserRoleClaims.AnyAsync() ? await _context.UserRoleClaims.MaxAsync(z => z.Id) + 1 : 0;
                    UserRoleClaim urc = new()
                    {
                        Id = urcid,
                        ClaimsId = item.Id,
                        UserRoleId = id,
                        IsRevoked = item.IsRevoked,
                    };
                    await _context.UserRoleClaims.AddAsync(urc);
                }
                await _context.SaveChangesAsync();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel
                    {
                        message = "کاربر با موفقیت ویرایش شد."
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
        [HttpDelete("role")]
        public async Task<IActionResult> DeleteUserRole(bool delete, int id, List<int> years)
        {
            try
            {
                var ur = await _context.UserRoles.Include(x => x.UserRoleClaims).Include(x => x.Years).SingleOrDefaultAsync(x => x.Id == id);
                if (ur != null)
                {
                    if (delete == true)
                    {
                        foreach (var year in years)
                        {
                            var y = ur.Years.SingleOrDefault(y => y.Id == year);
                            ur.Years.Remove(y);
                        }
                    }
                    else
                    {
                        ur.Years.Clear();
                    }
                    _context.UserRoleClaims.RemoveRange(ur.UserRoleClaims);
                    if (delete == true && ur.Years.Count() == 0)
                        _context.UserRoles.Remove(ur);
                    else if (ur.Years.Count() == 0)
                    {
                        ur.EndDate = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();
                    return Ok(new DTOResourceModel
                    {
                        status = 200,
                        message = "success",
                        data = new DataResourceModel()
                        {
                            message = "نقش کاربر مورد نظر با موفقیت حذف شد."
                        },
                        errors = new List<string>(),
                        warning = new List<string>()
                    });
                }
                return NotFound(new DTOResourceModel
                {
                    status = 404,
                    message = "not found",
                    data = new DataResourceModel()
                    {
                        message = "نقش مورد نظر برای این کگاربر پیدا نشد."
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
        #endregion
        #region user claim
        [HttpGet("claims")]
        public async Task<IActionResult> UserClaims(int userid)
        {
            try
            {
                var Claimsa = await _context.UserRoleClaims.Where(x => x.UserRole.UserId == userid).ToListAsync();
                List<ClaimResourceModel> Claims = await _context.UserRoleClaims.Where(x => x.UserRole.UserId == userid)
                    .Select(x => new ClaimResourceModel
                    {
                        Id = (int)x.ClaimsId,
                        FaName = x.Claims.FaName,
                        EnName = x.Claims.EnName,
                        IsRevoked = (bool)x.IsRevoked,
                        RoleId = (int)x.UserRole.RoleId,
                        OrganizationId = (int)x.UserRole.OrganizationId,
                        UserRoleClaimId = x.Id,
                        Years = (List<int>)x.Years.Select(x => x.Id)
                    })
                    .ToListAsync();
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel()
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
                    errors = new List<string>() { "خطایی در سرور رخ داد با پشتیبانی تماس بگیرید" },
                    warning = new List<string>()
                });
            }
        }
        [HttpPost("claim")]
        public async Task<IActionResult> UserClaim([FromBody] UserRoleBindingModel data)
        {
            try
            {
                foreach (var item in data.Claims)
                {
                    int urcid = await _context.UserRoleClaims.AnyAsync() ? await _context.UserRoleClaims.MaxAsync(z => z.Id) + 1 : 1;
                    int urid = await _context.UserRoles
                        .Where(x => x.UserId == data.UserId && x.OrganizationId == data.OrganizationId && x.RoleId == data.RoleId)?
                        .Select(x => x.Id)  // فقط id را انتخاب کنید
                        .SingleOrDefaultAsync();
                    UserRoleClaim urc = new()
                    {
                        Id = urcid,
                        ClaimsId = item.Id,
                        UserRoleId = urid + 1,
                        IsRevoked = item.IsRevoked,
                    };
                    if (item.Years != null)
                    {
                        foreach (var y in item.Years)
                        {
                            var year = await _context.Years.SingleOrDefaultAsync(x => x.Id == y);
                            if (year != null)
                                urc.Years.Add(year);
                        }
                    }
                    await _context.UserRoleClaims.AddAsync(urc);
                    await _context.SaveChangesAsync();
                }
                return Ok(new DTOResourceModel
                {
                    status = 200,
                    message = "success",
                    data = new DataResourceModel()
                    {
                        message = $"دسترسی {(data.Claims.Count() == 1 ? "" : "های")} .جدید به کاربر اضافه شد"
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
        [HttpDelete("claim")]
        public async Task<IActionResult> DeleteUserClaim(int claimsid, int userroleid, bool isrevoked)
        {
            UserRoleClaim urc = await _context.UserRoleClaims.Include(x => x.Years).SingleOrDefaultAsync(x => x.ClaimsId == claimsid && x.UserRoleId == userroleid && x.IsRevoked == isrevoked);
            if (urc != null)
            {
                try
                {
                    urc.Years.Clear();
                    _context.UserRoleClaims.Remove(urc);
                    await _context.SaveChangesAsync();
                    return Ok(new DTOResourceModel
                    {
                        status = 200,
                        message = "success",
                        data = new DataResourceModel()
                        {
                            message = "دسترسی مورد نظر با موفقیت حذف شد."
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
            return NotFound(new DTOResourceModel
            {
                status = 404,
                message = "not found",
                data = new DataResourceModel(),
                errors = new List<string>() { "دسترسی مورد نظر پیدا نشد." },
                warning = new List<string>()
            });
        }
        #endregion
    }
}
