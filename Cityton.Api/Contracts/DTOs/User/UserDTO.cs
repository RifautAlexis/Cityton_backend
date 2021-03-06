﻿using Cityton.Api.Data;
using System.Collections.Generic;

namespace Cityton.Api.Contracts.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public Role Role { get; set; }
        public string Token { get; set; }
        public int? GroupId { get; set; }
        public List<int> GroupIdsRequested { get; set; }

    }
}
