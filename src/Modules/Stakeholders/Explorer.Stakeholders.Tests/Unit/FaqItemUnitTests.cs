using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain.Help;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Unit
{
    public class FaqItemUnitTests
    {
        [Fact]
        public void Creates_faq_item()
        {
            var faq = new FaqItem("Payments", "Is payment secure?", "Yes, we use secure providers.");

            faq.Category.ShouldBe("Payments");
            faq.Question.ShouldBe("Is payment secure?");
            faq.Answer.ShouldBe("Yes, we use secure providers.");
            faq.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void Updates_question_and_answer()
        {
            var faq = new FaqItem("Payments", "Q1", "A1");

            faq.Update("Q2", "A2");

            faq.Question.ShouldBe("Q2");
            faq.Answer.ShouldBe("A2");
        }

        [Fact]
        public void Deactivates_faq()
        {
            var faq = new FaqItem("Payments", "Q", "A");

            faq.Deactivate();

            faq.IsActive.ShouldBeFalse();
        }
    }
}
