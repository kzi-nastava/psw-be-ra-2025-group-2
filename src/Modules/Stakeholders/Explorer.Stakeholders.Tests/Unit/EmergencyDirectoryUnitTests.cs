using Explorer.Stakeholders.Core.Domain.Emergency;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Unit
{
    public class EmergencyDirectoryUnitTests
    {
        [Fact]
        public void Adds_place_and_filters_by_type()
        {
            var dir = new EmergencyDirectory(new CountryCode("RS"), "instr", "disc");

            dir.AddPlace(EmergencyPlaceType.Hospital, "Bolnica 1", "Adresa 1", "123");
            dir.AddPlace(EmergencyPlaceType.PoliceStation, "Policija 1", "Adresa 2", null);

            dir.GetHospitals().Count.ShouldBe(1);
            dir.GetPoliceStations().Count.ShouldBe(1);

            dir.GetHospitals()[0].Name.ShouldBe("Bolnica 1");
            dir.GetPoliceStations()[0].Name.ShouldBe("Policija 1");
        }

        [Fact]
        public void Does_not_allow_duplicate_place()
        {
            var dir = new EmergencyDirectory(new CountryCode("RS"), "instr", "disc");

            dir.AddPlace(EmergencyPlaceType.Hospital, "Bolnica 1", "Adresa 1", null);

            Should.Throw<InvalidOperationException>(() =>
                dir.AddPlace(EmergencyPlaceType.Hospital, "Bolnica 1", "Adresa 1", null));
        }
    }
}
