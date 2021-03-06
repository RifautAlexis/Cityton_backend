using System.Collections.Generic;
using System.Linq;
using Cityton.Api.Contracts.DTOs;
using Cityton.Api.Data.Models;

namespace Cityton.Api.Contracts.Mappers
{
    public static class MessageMapper
    {

        public static MessageDTO ToDTO(this Message data)
        {
            if (data == null) return null;

            return new MessageDTO
            {
                Id = data.Id,
                Content = data.Content == null ? null : data.Content,
                Media = data.Media == null ? null : new MediaDTO { Id = data.Media.Id, Url = data.Media.Location, CreatedAt = data.Media.CreatedAt },
                Author = data.Author.ToUserMinimalDTO(),
                CreatedAt = data.CreatedAt,
                DiscussionId = data.DiscussionId,
            };
        }

        public static IEnumerable<MessageDTO> ToDTO(this IEnumerable<Message> data)
        {
            return data.Select(m => m.ToDTO()).ToList();
        }
    }
}
