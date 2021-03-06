using Microsoft.AspNetCore.Mvc;

namespace Cityton.Api.Contracts.DTOs
{
    public class CreateMessageDTO
    {
        [FromQuery]
        public string Message { get; set; }
        [FromQuery]
        public int DiscussionId { get; set; }
        [FromQuery]
        public string MediaUrl { get; set; }

        internal void Deconstruct(out string message, out int discussionId, out string mediaUrl)
        {
            message = Message;
            discussionId = DiscussionId;
            mediaUrl = MediaUrl;
        }

    }

}