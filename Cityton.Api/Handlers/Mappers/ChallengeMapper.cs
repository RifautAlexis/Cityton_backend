using System.Linq;
using Cityton.Api.Contracts.DTOs;

namespace Cityton.Api.Handlers.Mappers
{
    public static class ChallengeMapper
    {
        public static ChallengeDTO ToDTO(this Data.Models.Challenge data, int totalUsers)
        {
            if (data == null) return null;

            return new ChallengeDTO
            {
                Id = data.Id,
                Statement = data.Statement,
                Title = data.Title,
                CreatedAt = data.CreatedAt,
                SuccesRate = (data.Achievements.Count() / totalUsers) * 100
            };
        }
    }
}
