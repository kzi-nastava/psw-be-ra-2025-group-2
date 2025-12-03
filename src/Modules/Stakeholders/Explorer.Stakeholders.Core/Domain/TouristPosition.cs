using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class TouristPosition : Entity
    {
        public long TouristId { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set;}


        public TouristPosition(long touristId, double latitude, double longitude)
        {
            TouristId = touristId;
            Latitude = latitude;
            Longitude = longitude;
            Validate();
        }

        public void Update(long touristId, double latitude, double longitude)
        {
            TouristId = touristId;
            Latitude = latitude;
            Longitude = longitude;
            Validate();
        }

        private void Validate()
        {
            if (Latitude < -90 || Latitude > 90)
                throw new EntityValidationException("Latitude is between -90 and +90 degrees.");
            if (Longitude < -180 || Longitude > 180)
                throw new EntityValidationException("Longitude is between -180 and +180 degrees.");
        }
    }
}
