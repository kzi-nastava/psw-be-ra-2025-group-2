using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos.Quizzes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IQuizService
    {
        // Author functions
        public PagedResult<QuizDto> GetPagedByAuthor(long authorId, int page, int pageSize);
        public QuizDto Create(QuizDto quiz);
        public QuizDto Update(QuizDto quiz);
        public void Publish(long authorId, long quizId);
        public void Delete(long authorId, long quizId);

        // Tourist functions
        public PagedResult<QuizDto> GetPagedBlanksByAuthor(long authorId, int page, int pageSize);
        public PagedResult<QuizDto> GetPagedBlanks(int page, int pageSize);
        public QuizDto GetAnswered(long quizId);

        // Common
        public int GetPageCount(int pageSize);
        public int GetPageCountByAuthor(long authorId, int pageSize);
    }
}
