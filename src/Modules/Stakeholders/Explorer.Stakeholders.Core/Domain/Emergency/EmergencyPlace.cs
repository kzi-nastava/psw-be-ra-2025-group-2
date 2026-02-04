using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.Emergency
{

    public enum EmergencyPlaceType
    {
        Hospital = 0,
        PoliceStation = 1,
        FireStation = 2
    }

    public class EmergencyPlace : Entity
    {
        public long DirectoryId { get; private set; } 
        public EmergencyPlaceType Type { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string? Phone { get; private set; }

        private EmergencyPlace() { } // EF

        internal EmergencyPlace(EmergencyPlaceType type, string name, string address, string? phone)
        {
           
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException("Address cannot be empty.");

            
            Type = type;
            Name = name.Trim();
            Address = address.Trim();
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        }

        public void UpdateContact(string? phone)
        {
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        }
    }
}
