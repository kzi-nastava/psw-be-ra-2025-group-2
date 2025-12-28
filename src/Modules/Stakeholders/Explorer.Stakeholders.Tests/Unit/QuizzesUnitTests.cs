using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain.Quizzes;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Unit
{
    public class QuizzesUnitTests
    {
        [Fact]
        public void Creates()
        {
            var quiz = new Quiz(1, "Koji je glavni grad Srbije?");

            quiz.AuthorId.ShouldBe(1);
            quiz.QuestionText.ShouldBe("Koji je glavni grad Srbije?");
            quiz.AvailableOptions.Count.ShouldBe(0);
            quiz.IsPublished.ShouldBeFalse();
        }

        [Fact]
        public void Adds_options()
        {
            var quiz = new Quiz(1, "Koji je glavni grad Srbije?");
            quiz.AddOption(new QuizOption(0, "Beograd", "Beograd jeste glavni grad Srbije.", true));
            quiz.AddOption(new QuizOption(0, "Novi Sad", "Ovo nije tacno.", false));

            Assert.True(quiz.AvailableOptions[0].Equals(new QuizOption(1, "Beograd", "Beograd jeste glavni grad Srbije.", true)));
            Assert.True(quiz.AvailableOptions[1].Equals(new QuizOption(2, "Novi Sad", "Ovo nije tacno.", false)));
        }

        [Fact]
        public void Publishes()
        {
            var quiz = new Quiz(1, "Koji je glavni grad Srbije?");
            Should.Throw<InvalidOperationException>(quiz.Publish);

            quiz.AddOption(new QuizOption(0, "Novi Sad", "Ovo nije tacno.", false));
            Should.Throw<InvalidOperationException>(quiz.Publish);

            quiz.IsPublished.ShouldBeFalse();

            quiz.AddOption(new QuizOption(0, "Beograd", "Beograd jeste glavni grad Srbije.", true));

            quiz.Publish();
            quiz.IsPublished.ShouldBeTrue();
        }

        [Fact]
        public void Clears_options()
        {
            var quiz = new Quiz(1, "Koji je glavni grad Srbije?");
            quiz.AddOption(new QuizOption(0, "Beograd", "Beograd jeste glavni grad Srbije.", true));
            quiz.AddOption(new QuizOption(0, "Novi Sad", "Ovo nije tacno.", false));

            Assert.True(quiz.AvailableOptions[0].Equals(new QuizOption(1, "Beograd", "Beograd jeste glavni grad Srbije.", true)));
            Assert.True(quiz.AvailableOptions[1].Equals(new QuizOption(2, "Novi Sad", "Ovo nije tacno.", false)));

            quiz.ClearOptions();

            quiz.AvailableOptions.Count.ShouldBe(0);
        }

        [Fact]
        public void Published_quiz_is_immutable()
        {
            var quiz = new Quiz(1, "Koji je glavni grad Srbije?");
            quiz.AddOption(new QuizOption(0, "Beograd", "Beograd jeste glavni grad Srbije.", true));
            quiz.AddOption(new QuizOption(0, "Novi Sad", "Ovo nije tacno.", false));

            quiz.Publish();

            Should.Throw<InvalidOperationException>(() => quiz.ChangeQuestionText("AAAAAA"));
            Should.Throw<InvalidOperationException>(() => quiz.AddOption(new QuizOption(0, "Zrenjanin", "Ni ovo.", false)));
            Should.Throw<InvalidOperationException>(quiz.ClearOptions);
        }
    }
}
