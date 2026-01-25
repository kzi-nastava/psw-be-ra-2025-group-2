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
    }
}
