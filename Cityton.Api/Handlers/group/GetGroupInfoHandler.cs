using System.Linq;
using System.Threading.Tasks;
using Cityton.Api.Data;
using Cityton.Api.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cityton.Api.Contracts.Requests;
using Cityton.Api.Contracts.DTOs;
using Cityton.Api.Contracts.Mappers;

namespace Cityton.Api.Handlers
{
    public class GetGroupInfoHandler : IHandler<GetGroupInfoRequest, ObjectResult>
    {
        private readonly ApplicationDBContext _appDBContext;

        public GetGroupInfoHandler(ApplicationDBContext appDBContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDBContext = appDBContext;
        }

        public async Task<ObjectResult> Handle(GetGroupInfoRequest request)
        {
            int groupId = request.Id;
            Group group = await _appDBContext.Groups
                .Where(g => g.Id == request.Id)
                .Include(g => g.Supervisor)
                .Include(g => g.Members)
                .ThenInclude(pg => pg.User)
                .FirstOrDefaultAsync();

            if (group == null) { return new NotFoundObjectResult("No user found"); }

            Company company = await _appDBContext.Companies
                .Where(c => c.Id == 1)
                .FirstOrDefaultAsync();

            GroupDTO groupInfo = group.ToDTO(company.MinGroupSize, company.MaxGroupSize);

            return new OkObjectResult(groupInfo);
        }
    }
}