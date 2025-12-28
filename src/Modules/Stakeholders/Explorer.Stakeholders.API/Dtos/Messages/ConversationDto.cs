using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Messages
{
    public class ConversationDto
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageAt { get; set; }
    }
}
