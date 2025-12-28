using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain.Quizzes;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class QuizDbRepository : IQuizRepository
    {
        private readonly StakeholdersContext _dbContext;

        public QuizDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Quiz Create(Quiz quiz)
        {
            _dbContext.Quizzes.Add(quiz);
            _dbContext.SaveChanges();

            return quiz;
        }

        public void Delete(long quizId)
        {
            var entity = _dbContext.Quizzes.Find(quizId);

            if(entity == null)
            {
                throw new NotFoundException($"Not found: {quizId}");
            }

            _dbContext.Quizzes.Remove(entity);
            _dbContext.SaveChanges();
        }

        public Quiz? GetById(long id)
        {
            return _dbContext.Quizzes.Find(id);
        }

        public long GetCount()
        {
            return _dbContext.Quizzes.Count();
        }

        public long GetCountByAuthor(long authorId)
        {
            return _dbContext.Quizzes.Count(q => q.AuthorId == authorId);
        }
        public long GetCountPublished()
        {
            return _dbContext.Quizzes.Count(q => q.IsPublished == true);
        }
        public long GetCountByAuthorPublished(long authorId)
        {
            return _dbContext.Quizzes.Count(q => q.AuthorId == authorId && q.IsPublished == true);
        }

        public PagedResult<Quiz> GetPaged(int page, int pageSize)
        {
            var task = _dbContext.Quizzes.GetPaged(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public PagedResult<Quiz> GetPagedByAuthor(long authorId, int page, int pageSize)
        {
            var task = _dbContext.Quizzes.Where(q => q.AuthorId == authorId).GetPaged(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public PagedResult<Quiz> GetPagedPublished(int page, int pageSize)
        {
            var task = _dbContext.Quizzes.Where(q => q.IsPublished == true).GetPaged(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public PagedResult<Quiz> GetPagedByAuthorPublished(long authorId, int page, int pageSize)
        {
            var task = _dbContext.Quizzes.Where(q => q.AuthorId == authorId && q.IsPublished == true).GetPaged(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public Quiz Update(Quiz quiz)
        {
            try
            {
                _dbContext.Update(quiz);
                _dbContext.SaveChanges();
            }
            catch(DbUpdateException ex)
            {
                throw new NotFoundException(ex.Message);
            }

            return quiz;
        }
    }
}
