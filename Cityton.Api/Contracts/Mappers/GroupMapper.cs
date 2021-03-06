using System.Collections.Generic;
using System.Linq;
using Cityton.Api.Contracts.DTOs;
using Cityton.Api.Data;
using Cityton.Api.Data.Models;

namespace Cityton.Api.Contracts.Mappers
{
    public static class GroupMapper
    {
        public static GroupDTO ToDTO(this Group data, int minGroupSize, int maxGroupSize)
        {
            if (data == null) return null;

            return new GroupDTO
            {
                Id = data.Id,
                Name = data.Name,
                Creator = data.Members.Where(pg => pg.IsCreator == true).Select(pg => pg.User.ToUserMinimalDTO()).FirstOrDefault(),
                Members = data.Members.Where(pg => pg.IsCreator == false && pg.Status == Status.Accepted).ToList().ToParticipantGroupMinimalDTO(),
                RequestsAdhesion = data.Members.Where(pg => pg.IsCreator == false && pg.Status == Status.Waiting).ToList().ToParticipantGroupMinimalDTO(),
                HasReachMinSize = data.Members.Where(pg => pg.Status == Status.Accepted).Count() >= minGroupSize,
                HasReachMaxSize = data.Members.Where(pg => pg.Status == Status.Accepted).Count() == maxGroupSize,
                Supervisor = data.Supervisor == null ? null : data.Supervisor.ToUserMinimalDTO()
            };
        }

        public static GroupMinimalDTO ToGroupMinimalDTO(this Group data, int minGroupSize, int maxGroupSize)
        {
            if (data == null) return null;

            return new GroupMinimalDTO
            {
                Id = data.Id,
                Name = data.Name,
                HasReachMinSize = data.Members.Count >= minGroupSize,
                HasReachMaxSize = data.Members.Count == maxGroupSize,
                Supervisor = data.Supervisor == null ? null : data.Supervisor.ToUserMinimalDTO()
            };
        }

        public static List<GroupMinimalDTO> ToGroupMinimalDTO(this List<Group> data, int minGroupSize, int maxGroupSize)
        {
            return data.Select(d => d.ToGroupMinimalDTO(minGroupSize, maxGroupSize)).ToList();
        }
    }
}
