using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.Quizzes
{
    public class Quiz : AggregateRoot
    {
        public long AuthorId { get; private set; }
        public string QuestionText { get; private set; }

        private readonly List<QuizOption> _availableOptions = new();
        public IReadOnlyList<QuizOption> AvailableOptions => _availableOptions.AsReadOnly();

        public bool IsPublished { get; private set; }

        private Quiz() { }

        public Quiz(long authorId, string questionText)
        {
            authorId = AuthorId;
            ChangeQuestionText(questionText);
        }

        public void ChangeQuestionText(string questionText)
        {
            if (IsPublished)
            {
                throw new InvalidOperationException("Cannot edit question text of a published quiz.");
            }

            if(questionText == null)
            {
                throw new ArgumentException("Question text cannot be null.");
            }

            QuestionText = questionText;
        }

        public void Publish()
        {
            if(_availableOptions.Count < 1)
            {
                throw new InvalidOperationException("Cannot publish a quiz with less than 1 possible answer.");
            }

            if(!_availableOptions.Where(o => o.IsCorrect).Any())
            {
                throw new InvalidOperationException("Cannot publish a quiz with no correct answers.");
            }

            IsPublished = true;
        }
        public void AddOption(QuizOption optionData)
        {
            if (IsPublished)
            {
                throw new InvalidOperationException("Cannot add a new answer to a published quiz.");
            }

            var newOption = new QuizOption(GetLastAnswerOrdinal() + 1, optionData.OptionText, optionData.Explanation, optionData.IsCorrect);

            _availableOptions.Add(newOption);
        }

        public void UpdateOption(QuizOption option)
        {
            if (IsPublished)
            {
                throw new InvalidOperationException("Cannot update an answer of a published quiz.");
            }

            var existing = _availableOptions.FirstOrDefault(o => o.Ordinal == option.Ordinal);

            if (existing == null)
            {
                throw new InvalidOperationException($"Answer with ordinal {option.Ordinal} does not exist in this quiz.");
            }

            existing = new QuizOption(option.Ordinal, option.OptionText, option.Explanation, option.IsCorrect);
        }

        public void RemoveOption(int ordinal)
        {
            var existing = _availableOptions.Where(o => o.Ordinal == ordinal).FirstOrDefault();

            if(existing == null)
            {
                throw new ArgumentException($"Answer with ordinal {ordinal} does not exist in this quiz.");
            }

            _availableOptions.Remove(existing);
            RecalculateOrdinals();
        }

        public void ClearOptions()
        {
            _availableOptions.Clear();
        }

        public bool IsAnswerCorrect(int ordinal, bool isMarkedTrue)
        {
            if(ordinal <= 0 || ordinal > GetLastAnswerOrdinal())
            {
                throw new ArgumentOutOfRangeException(nameof(ordinal));
            }

            var option = _availableOptions.Where(o => o.Ordinal == ordinal).FirstOrDefault();

            if(option == null)
            {
                throw new InvalidDataException($"Option with ordinal {ordinal} not found.");
            }

            return isMarkedTrue == option.IsCorrect;
        }


        private int GetLastAnswerOrdinal()
        {
            var last = _availableOptions.LastOrDefault();

            return last == null ? 0 : last.Ordinal;
        }

        private void RecalculateOrdinals()
        {
            for(int i = 0; i < _availableOptions.Count; i++)
            {
                _availableOptions[i] = new QuizOption(i + 1, _availableOptions[i].OptionText, _availableOptions[i].Explanation, _availableOptions[i].IsCorrect);
            }
        }
    }
}
