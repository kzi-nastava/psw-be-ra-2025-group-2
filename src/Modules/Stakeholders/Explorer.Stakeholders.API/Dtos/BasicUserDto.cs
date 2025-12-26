using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class BasicUserDto
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
