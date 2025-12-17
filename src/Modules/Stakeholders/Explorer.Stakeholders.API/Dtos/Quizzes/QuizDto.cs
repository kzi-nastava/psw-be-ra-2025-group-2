using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Quizzes
{
    public class QuizDto
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public string QuestionText { get; set; }
        public List<QuizOptionDto> AvailableOptions { get; set; }
        public bool IsPublished { get; set; }
    }
}
