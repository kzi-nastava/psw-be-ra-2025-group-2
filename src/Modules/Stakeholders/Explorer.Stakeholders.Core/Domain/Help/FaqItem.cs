using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.Help
{
    public class FaqItem : AggregateRoot
    {
        public string Category { get; private set; }
        public string Question { get; private set; }
        public string Answer { get; private set; }
        public bool IsActive { get; private set; }

        public FaqItem(string category, string question, string answer)
        {
            Category = category;
            Question = question;
            Answer = answer;
            IsActive = true;
        }

        public void Update(string question, string answer) 
        { 
            Question = question;
            Answer = answer;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
