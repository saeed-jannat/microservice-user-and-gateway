using identity.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using identity.Core.Application.DTOs;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using identity.Presentation.Models.ResourceModel;
using Microsoft.EntityFrameworkCore;
using identity.Infrastructure.Data;

namespace identity.Core.Application.Services
{
    public class Initialize : IInitialize
    {
        private readonly SilbargBaseIdentityContext _context;
        public Initialize(SilbargBaseIdentityContext context)
        {
            _context=context;
        }
        public async Task SendClaimToApiGAteway()
        {
            var res = await _context.Claims.AsNoTracking().Include(x => x.InverseParent).Where(x => x.ParentId == null).Select(x => new Claim1ResourceModel
            {
                Id = x.Id,
                FaName = x.FaName,
                EnName = x.EnName,
                Path = x.Path,
                IsActive = x.IsActive,
                ParentId = x.ParentId,
                Verb = x.Verb,
                IsCommon = x.IsCommon,
                InverseParent = x.InverseParent.Select(m => new Claim1ResourceModel
                {
                    Id = m.Id,
                    FaName = m.FaName,
                    EnName = m.EnName,
                    Path = m.Path,
                    IsActive = m.IsActive,
                    ParentId = m.ParentId,
                    Verb = m.Verb,
                    InverseParent = null,
                    IsCommon = m.IsCommon,
                })
            }).ToListAsync();
            var json = JsonSerializer.Serialize(res);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient httpClient =new HttpClient();
            var responseUserRoles = await httpClient.PostAsync($"{StaticData.ApiGateWayBaseUrl}/api/setting/set-claims", content);

        }

    }
}
