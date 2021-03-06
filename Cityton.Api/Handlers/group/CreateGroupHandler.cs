using System;
using System.Threading.Tasks;
using Cityton.Api.Data;
using Cityton.Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Cityton.Api.Contracts.Requests;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Cityton.Api.Handlers
{
    public class CreateGroupHandler : IHandler<CreateGroupRequest, ObjectResult>
    {
        private readonly ApplicationDBContext _appDBContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateGroupHandler(ApplicationDBContext appDBContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDBContext = appDBContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ObjectResult> Handle(CreateGroupRequest request)
        {
            string name = request.createGroupDTO.Name;
            int currentUserId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value);


            Group group = new Group
            {
                Name = name,
                CreatedAt = DateTime.Now,
                SupervisorId = null
            };

            await _appDBContext.Groups.AddAsync(group);
            await _appDBContext.SaveChangesAsync();

            ParticipantGroup participantGroups = new ParticipantGroup
            {
                IsCreator = true,
                Status = Status.Accepted,
                BelongingGroupId = group.Id,
                UserId = currentUserId
            };

            await _appDBContext.ParticipantGroups.AddAsync(participantGroups);
            await _appDBContext.SaveChangesAsync();

            Discussion discussion = new Discussion
            {
                CreatedAt = DateTime.Now,
                Name = name,
                GroupId = group.Id
            };

            await _appDBContext.Discussions.AddAsync(discussion);
            await _appDBContext.SaveChangesAsync();

            UserInDiscussion userInDiscussion = new UserInDiscussion
            {
                JoinedAt = DateTime.Now,
                DiscussionId = discussion.Id,
                ParticipantId = currentUserId
            };

            await _appDBContext.UsersInDiscussion.AddAsync(userInDiscussion);
            await _appDBContext.SaveChangesAsync();

            List<ParticipantGroup> requestsToDelete = await _appDBContext.ParticipantGroups
                .Where(pg => pg.Status == Status.Waiting)
                .ToListAsync();

            _appDBContext.ParticipantGroups.RemoveRange(requestsToDelete);
            await _appDBContext.SaveChangesAsync();

            return new OkObjectResult(true);
        }
    }
}