using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.Quizzes
{
    public class QuizAnswerDto
    {
        public int Ordinal { get; set; }
        public bool IsMarkedTrue { get; set; }
    }
}
