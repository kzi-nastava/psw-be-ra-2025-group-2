
using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class TouristEquipment : Entity
    {
        public int TouristId { get; set; }
        public List<int> Equipment { get; set; }
        public TouristEquipment()
        {
            Equipment = new List<int>();
        }
        public TouristEquipment(int touristId, List<int> equipment)
        {
            TouristId = touristId;
            Equipment = equipment;
        }
    }
}
