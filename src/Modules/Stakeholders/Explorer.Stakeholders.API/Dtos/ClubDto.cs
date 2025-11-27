using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubDto
    {
        public long Id { get; set; }          
        public string Name { get; set; }
        public string Description { get; set; }
        public long OwnerId { get; set; }     
        public List<string> ImageUrls { get; set; } = new();
    }
}
