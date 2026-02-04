using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.Emergency
{
    public class EmergencyDirectory : AggregateRoot
    {
        public CountryCode Country { get; private set; }
        public string Instructions { get; private set; }
        public string Disclaimer { get; private set; }

        private readonly List<EmergencyPlace> _places = new();
        public IReadOnlyCollection<EmergencyPlace> Places => _places.AsReadOnly();
        
        private readonly List<Embassy> _embassies = new();
        public IReadOnlyCollection<Embassy> Embassies => _embassies.AsReadOnly();

        private readonly List<EmergencyPhrase> _phrases = new();
        public IReadOnlyCollection<EmergencyPhrase> Phrases => _phrases.AsReadOnly();

        


        private EmergencyDirectory() { } // EF

        public EmergencyDirectory(CountryCode country, string instructions, string disclaimer)
        {
            Country = country ?? throw new ArgumentNullException(nameof(country));

            if (string.IsNullOrWhiteSpace(instructions))
                throw new ArgumentException("Instructions cannot be empty.");
            if (string.IsNullOrWhiteSpace(disclaimer))
                throw new ArgumentException("Disclaimer cannot be empty.");

            Instructions = instructions.Trim();
            Disclaimer = disclaimer.Trim();
        }

        
        public List<EmergencyPlace> GetHospitals()
            => _places.Where(p => p.Type == EmergencyPlaceType.Hospital).ToList();

        public List<EmergencyPlace> GetPoliceStations()
            => _places.Where(p => p.Type == EmergencyPlaceType.PoliceStation).ToList();

        public List<EmergencyPlace> GetFireStations()
           => _places.Where(p => p.Type == EmergencyPlaceType.FireStation).ToList();

        public void UpdateTexts(string instructions, string disclaimer)
        {
            if (string.IsNullOrWhiteSpace(instructions))
                throw new ArgumentException("Instructions cannot be empty.");
            if (string.IsNullOrWhiteSpace(disclaimer))
                throw new ArgumentException("Disclaimer cannot be empty.");

            Instructions = instructions.Trim();
            Disclaimer = disclaimer.Trim();
        }

        public void AddPlace(EmergencyPlaceType type, string name, string address, string? phone)
        {
            var exists = _places.Any(p =>
                p.Type == type &&
                p.Name.Equals(name?.Trim() ?? "", StringComparison.OrdinalIgnoreCase) &&
                p.Address.Equals(address?.Trim() ?? "", StringComparison.OrdinalIgnoreCase));

            if (exists)
                throw new InvalidOperationException("Emergency place already exists.");

            var place = new EmergencyPlace(type, name, address, phone);
            _places.Add(place);
        }


        public void RemovePlace(long placeId)
        {
            var place = _places.FirstOrDefault(p => p.Id == placeId);
            if (place == null) throw new KeyNotFoundException("Emergency place not found.");
            _places.Remove(place);
        }

        public void EnsureCountryMatches(CountryCode code)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));
            if (!Country.Equals(code))
                throw new InvalidOperationException("Country code mismatch.");
        }


        public List<EmergencyPhrase> GetPhrases(EmergencyPhraseCategory category)
            => _phrases.Where(p => p.Category == category).ToList();

        public void AddEmbassy(string name, string address, string? phone, string? email, string? website)
        {
            var exists = _embassies.Any(e =>
                e.Name.Equals(name?.Trim() ?? "", StringComparison.OrdinalIgnoreCase) &&
                e.Address.Equals(address?.Trim() ?? "", StringComparison.OrdinalIgnoreCase));
            if (exists) throw new InvalidOperationException("Embassy already exists.");

            _embassies.Add(new Embassy(name, address, phone, email, website));
        }

        public void AddPhrase(EmergencyPhraseCategory category, string myText, string localText)
        {
            var exists = _phrases.Any(p =>
                p.Category == category &&
                p.MyText.Equals(myText?.Trim() ?? "", StringComparison.OrdinalIgnoreCase) &&
                p.LocalText.Equals(localText?.Trim() ?? "", StringComparison.OrdinalIgnoreCase));
            if (exists) throw new InvalidOperationException("Phrase already exists.");

            _phrases.Add(new EmergencyPhrase(category, myText, localText));
        }

        public void RemoveEmbassy(long embassyId)
        {
            var e = _embassies.FirstOrDefault(x => x.Id == embassyId);
            if (e == null) throw new KeyNotFoundException("Embassy not found.");
            _embassies.Remove(e);
        }

        public void RemovePhrase(long phraseId)
        {
            var p = _phrases.FirstOrDefault(x => x.Id == phraseId);
            if (p == null) throw new KeyNotFoundException("Phrase not found.");
            _phrases.Remove(p);
        }

    }
}
