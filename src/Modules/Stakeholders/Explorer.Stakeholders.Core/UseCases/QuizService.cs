using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos.Quizzes;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.Quizzes;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    internal class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public QuizService(IQuizRepository quizRepository, IUserRepository userRepository, IMapper mapper)
        {
            _quizRepository = quizRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }
        
        // Author
        public PagedResult<QuizDto> GetPagedByAuthor(long authorId, int page, int pageSize)
        {
            var result = _quizRepository.GetPagedByAuthor(authorId, page, pageSize);
            var items = result.Results.Select(_mapper.Map<QuizDto>).ToList();

            return new PagedResult<QuizDto>(items, items.Count);
        }

        public QuizDto Create(QuizDto quiz)
        {
            var author = _userRepository.Get(quiz.AuthorId);

            if(author == null)
            {
                throw new InvalidDataException($"User data is not consistent: author with id {quiz.AuthorId} not found.");
            }

            var newQuiz = new Quiz(quiz.AuthorId, quiz.QuestionText);

            foreach(var option in quiz.AvailableOptions)
            {
                newQuiz.AddOption(new QuizOption(0, option.OptionText, option.Explanation ?? "", option.IsCorrect ?? false));
            }

            var result = _quizRepository.Create(newQuiz);

            return _mapper.Map<QuizDto>(result);
        }

        public QuizDto Update(QuizDto quiz)
        {
            var existing = _quizRepository.GetById(quiz.Id);

            if(existing == null)
            {
                throw new ArgumentException($"Quiz with id {quiz.Id} not found.");
            }

            existing.ChangeQuestionText(quiz.QuestionText);

            existing.ClearOptions();
            foreach(var option in quiz.AvailableOptions)
            {
                existing.AddOption(new QuizOption(0, option.OptionText, option.Explanation ?? "", option.IsCorrect ?? false));
            }

            var result = _quizRepository.Update(existing);

            return _mapper.Map<QuizDto>(result);
        }

        public void Delete(long authorId, long quizId)
        {
            var existing = _quizRepository.GetById(quizId);

            if(existing == null)
            {
                throw new ArgumentException($"Quiz with id {quizId} not found.");
            }

            if(existing.AuthorId != authorId)
            {
                throw new ForbiddenException("Unauthorized operation.");
            }

            _quizRepository.Delete(quizId);
        }

        public void Publish(long authorId, long quizId)
        {
            var existing = _quizRepository.GetById(quizId);

            if (existing == null)
            {
                throw new ArgumentException($"Quiz with id {quizId} not found.");
            }

            if (existing.AuthorId != authorId)
            {
                throw new ForbiddenException("Unauthorized operation.");
            }

            existing.Publish();

            _quizRepository.Update(existing);
        }

        // Tourist
        public PagedResult<QuizDto> GetPagedBlanks(int page, int pageSize)
        {
            var result = _quizRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<QuizDto>).ToList();

            foreach(var item in items)
            {
                foreach(var option in item.AvailableOptions)
                {
                    option.IsCorrect = null;
                    option.Explanation = null;
                }
            }

            return new PagedResult<QuizDto>(items, items.Count);
        }

        public PagedResult<QuizDto> GetPagedBlanksByAuthor(long authorId, int page, int pageSize)
        {
            var result = _quizRepository.GetPagedByAuthor(authorId, page, pageSize);
            var items = result.Results.Select(_mapper.Map<QuizDto>).ToList();

            foreach(var item in items)
            {
                foreach(var option in item.AvailableOptions)
                {
                    option.IsCorrect = null;
                    option.Explanation = null;
                }
            }

            return new PagedResult<QuizDto>(items, items.Count);
        }

        public QuizDto GetAnswered(long quizId)
        {
            var result = _quizRepository.GetById(quizId);

            if(result == null)
            {
                throw new ArgumentException($"Quiz with id {quizId} not found.");
            }

            return _mapper.Map<QuizDto>(result);
        }

        // Common
        public int GetPageCount(int pageSize)
        {
            var total = _quizRepository.GetCount();

            return (int)Math.Ceiling((double)total / pageSize);
        }

        public int GetPageCountByAuthor(long authorId, int pageSize)
        {
            var totalByAuthor = _quizRepository.GetCountByAuthor(authorId);

            return (int)Math.Ceiling((double)totalByAuthor / pageSize);
        }
    }
}
