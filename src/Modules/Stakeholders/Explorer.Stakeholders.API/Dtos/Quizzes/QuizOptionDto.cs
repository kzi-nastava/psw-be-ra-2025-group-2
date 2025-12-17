using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Quizzes
{
    public class QuizOptionDto
    {
        public int Ordinal { get; set; }
        public string OptionText { get; set; }
        public string? Explanation { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
