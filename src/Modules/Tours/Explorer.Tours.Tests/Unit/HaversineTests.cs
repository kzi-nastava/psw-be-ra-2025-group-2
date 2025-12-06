using Explorer.Tours.Core.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Unit
{
    public class HaversineTests
    {
        [Theory]
        [InlineData(45.251917, 19.836987, 45.254276, 19.842461, 495, 505)] // Raskrsnica: Futoska-Bul. Oslobodjenja, do raskrsnice Jevrejska-Bul. Mihajla Pupina
        [InlineData(68.44251, 17.41869, 68.44137, 17.42103, 158, 159)]
        public void Calculated_value_is_within_range(double lat1, double lon1, double lat2, double lon2, double expectedLowerRange, double expectedUpperRange)
        {
            // Arrange & Act
            var distance = Haversine.CalculateDistance(lat1, lon1, lat2, lon2);

            //Assert
            // Toleranicja od 1%
            Assert.True(distance > Distance.FromMeters(expectedLowerRange));
            Assert.True(distance < Distance.FromMeters(expectedUpperRange));

            Console.WriteLine(distance);
        }

    }
}
