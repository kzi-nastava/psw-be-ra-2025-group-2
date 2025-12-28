using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Messages
{
    public class SendMessageDto
    {
        public long ReceiverId { get; set; }
        public string Content { get; set; }
    }
}
