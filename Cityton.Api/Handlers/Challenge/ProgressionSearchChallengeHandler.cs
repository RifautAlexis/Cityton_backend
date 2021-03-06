using System;
using System.Linq;
using System.Threading.Tasks;
using Cityton.Api.Data;
using Cityton.Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cityton.Api.Contracts.Requests;
using Cityton.Api.Contracts.DTOs;
using System.Collections.Generic;
using Cityton.Api.Contracts.Mappers;

namespace Cityton.Api.Handlers
{
    public class ProgressionSearchChallengeHandler : IHandler<ProgressionSearchChallengeRequest, ObjectResult>
    {
        private readonly ApplicationDBContext _appDBContext;

        public ProgressionSearchChallengeHandler(ApplicationDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        public async Task<ObjectResult> Handle(ProgressionSearchChallengeRequest request)
        {
            (string searchText, int threadId) = request;

            StringComparison comparison = StringComparison.OrdinalIgnoreCase;

            List<Challenge> challenges = await _appDBContext.Challenges
                .Where(c => c.ChallengeGivens.All(cg => cg.ChallengedGroup.DiscussionId == threadId && cg.ChallengeId != c.Id) && (string.IsNullOrEmpty(searchText) || c.Title.Contains(searchText, comparison) || c.Statement.Contains(searchText, comparison)))
                .OrderBy(c => c.Title)
                .Include(c => c.Achievements)
                .ToListAsync();

            int totalUsers = await _appDBContext.Users.CountAsync();

            List<ChallengeDTO> challengesDTO = challenges.Select(c => c.ToDTO(totalUsers)).ToList();

            return new OkObjectResult(challengesDTO);
        }
    }
}