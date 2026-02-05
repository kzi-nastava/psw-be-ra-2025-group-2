using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubMessageDto
    {
        public long Id { get; set; }    
        public long SenderId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
