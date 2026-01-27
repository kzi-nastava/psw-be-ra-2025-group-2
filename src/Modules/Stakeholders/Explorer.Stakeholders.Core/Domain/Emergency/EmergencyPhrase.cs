using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.Emergency
{

    public enum EmergencyPhraseCategory
    {
        Medicine = 0,
        Police = 1
    }

    public class EmergencyPhrase : Entity
    {
        public long DirectoryId { get; private set; }
        public EmergencyPhraseCategory Category { get; private set; }
        public string MyText { get; private set; }
        public string LocalText { get; private set; }

        private EmergencyPhrase() { }

        internal EmergencyPhrase(EmergencyPhraseCategory category, string myText, string localText)
        {
            if (string.IsNullOrWhiteSpace(myText)) throw new ArgumentException("MyText cannot be empty.");
            if (string.IsNullOrWhiteSpace(localText)) throw new ArgumentException("LocalText cannot be empty.");

            Category = category;
            MyText = myText.Trim();
            LocalText = localText.Trim();
        }
    }
}


