using AutoMapper.Configuration.Conventions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain.Quizzes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IQuizRepository
    {
        public Quiz? GetById(long id);
        public PagedResult<Quiz> GetPaged(int page, int pageSize);
        public PagedResult<Quiz> GetPagedPublished(int page, int pageSize);
        public PagedResult<Quiz> GetPagedByAuthor(long authorId, int page, int pageSize);
        public PagedResult<Quiz> GetPagedByAuthorPublished(long authorId, int page, int pageSize);

        public Quiz Create(Quiz quiz);
        public Quiz Update(Quiz quiz);
        public void Delete(long quizId);

        public long GetCount();
        public long GetCountPublished();
        public long GetCountByAuthor(long authorId);
        public long GetCountByAuthorPublished(long authorId);
    }
}
