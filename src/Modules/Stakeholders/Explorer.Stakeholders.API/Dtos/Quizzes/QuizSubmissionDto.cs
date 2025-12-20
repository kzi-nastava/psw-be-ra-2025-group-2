using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Quizzes
{
    public class QuizSubmissionDto
    {
        public long TouristId { get; set; }
        public long QuizId { get; set; }
        public List<QuizAnswerDto> Answers { get; set; }
    }
}
