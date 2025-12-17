using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.Quizzes
{
    public class QuizOption : ValueObject
    {
        public int Ordinal { get; init; }
        public string OptionText { get; init; }
        public string Explanation { get; init; }
        public bool IsCorrect { get; init; }

        [JsonConstructor]
        public QuizOption(int ordinal, string optionText, string explanation, bool isCorrect)
        {
            if(ordinal < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ordinal));
            }

            if(optionText == null)
            {
                throw new ArgumentException("Answer text cannot be null.");
            }

            if(explanation == null)
            {
                throw new ArgumentException("Explanation text cannot be null.");
            }

            Ordinal = ordinal;
            OptionText = optionText;
            Explanation = explanation;
            IsCorrect = isCorrect;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Ordinal;
            yield return OptionText;
            yield return Explanation;
            yield return IsCorrect;
        }
    }
}
