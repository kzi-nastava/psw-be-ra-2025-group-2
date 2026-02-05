using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.Emergency
{
    public class Embassy : Entity
    {
        public long DirectoryId { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string? Phone { get; private set; }
        public string? Email { get; private set; }
        public string? Website { get; private set; }

        private Embassy() { }

        internal Embassy(string name, string address, string? phone, string? email, string? website)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException("Address cannot be empty.");

            Name = name.Trim();
            Address = address.Trim();
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
            Website = string.IsNullOrWhiteSpace(website) ? null : website.Trim();
        }
    }

}
