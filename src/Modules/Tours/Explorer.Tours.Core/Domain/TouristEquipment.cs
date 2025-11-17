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
        public int TouristId { get; init; }
        public List<int> Equipment {  get; init; }
        public TouristEquipment( int tourist) { }
        public TouristEquipment( int touristId, List<int> equipment) 
        {
            TouristId = touristId;
            Equipment = equipment;
        }
    }
}
