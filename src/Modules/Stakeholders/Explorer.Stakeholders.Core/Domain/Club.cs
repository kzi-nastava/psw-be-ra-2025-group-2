using System;
using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Club : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public long OwnerId { get; init; }
        public List<string> ImageUrls { get; private set; }

        public Club(string name, string description, long ownerId, List<string> imageUrls)
        {
            Name = name;
            Description = description;
            OwnerId = ownerId;
            ImageUrls = imageUrls ?? new List<string>();
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Invalid Name");

            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Invalid Description");

           // if (OwnerId <= 0)
            //throw new ArgumentException("Invalid OwnerId");

            if (ImageUrls == null || ImageUrls.Count == 0)
                throw new ArgumentException("At least one image is required");
        }

        public void Update(string name, string description, List<string> imageUrls)
        {
            Name = name;
            Description = description;
            ImageUrls = imageUrls ?? new List<string>();
            Validate();
        }
    }
}
