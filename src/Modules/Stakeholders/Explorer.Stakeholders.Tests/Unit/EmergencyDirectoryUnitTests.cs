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
            dir.AddPlace(EmergencyPlaceType.PoliceStation, "Police 1", "Adresa 2", null);
            dir.AddPlace(EmergencyPlaceType.FireStation, "Vatrogasci 1", "Adresa 3", "456");

            dir.GetHospitals().Count.ShouldBe(1);
            dir.GetPoliceStations().Count.ShouldBe(1);
            dir.GetFireStations().Count.ShouldBe(1);

            dir.GetHospitals()[0].Name.ShouldBe("Bolnica 1");
            dir.GetPoliceStations()[0].Name.ShouldBe("Police 1");
            dir.GetFireStations()[0].Name.ShouldBe("Vatrogasci 1");
        }

        [Fact]
        public void Does_not_allow_duplicate_place()
        {
            var dir = new EmergencyDirectory(new CountryCode("RS"), "instr", "disc");

            dir.AddPlace(EmergencyPlaceType.Hospital, "Bolnica 1", "Adresa 1", null);

            Should.Throw<InvalidOperationException>(() =>
                dir.AddPlace(EmergencyPlaceType.Hospital, "Bolnica 1", "Adresa 1", null));
        }


        [Fact]
        public void Adds_embassy_and_blocks_duplicates()
        {
            var dir = new EmergencyDirectory(new CountryCode("RS"), "instr", "disc");

            dir.AddEmbassy("Embassy 1", "Addr 1", "123", "a@b.com", "site");
            dir.Embassies.Count.ShouldBe(1);

            Should.Throw<InvalidOperationException>(() =>
                dir.AddEmbassy("Embassy 1", "Addr 1", "456", null, null));
        }

        [Fact]
        public void Adds_phrases_and_filters_by_category()
        {
            var dir = new EmergencyDirectory(new CountryCode("RS"), "instr", "disc");

            dir.AddPhrase(EmergencyPhraseCategory.Medicine, "Moje 1", "Local 1");
            dir.AddPhrase(EmergencyPhraseCategory.Police, "Moje 2", "Local 2");

            dir.GetPhrases(EmergencyPhraseCategory.Medicine).Count.ShouldBe(1);
            dir.GetPhrases(EmergencyPhraseCategory.Police).Count.ShouldBe(1);
        }

    }
}
