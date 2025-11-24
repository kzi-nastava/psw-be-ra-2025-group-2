using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;
using AutoMapper;
using Explorer.Stakeholders.API.Public;

namespace Explorer.Stakeholders.Core.UseCases;
public class AdminUserService : IAdminUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;
    public AdminUserService(IUserRepository userRepository, IPersonRepository personRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _personRepository = personRepository;
        _mapper = mapper;
    }

    public void CreateAccount(AccountRegistrationDto account)
    {
        try {
            User user;
            if (account.Role == "Administrator")
            {
                user = _userRepository.Create(new User(account.Username, account.Password, account.Email, UserRole.Administrator, true));
            }
            else if (account.Role == "Author")
            {
                user = _userRepository.Create(new User(account.Username, account.Password, account.Email, UserRole.Author, true));
            }
            else
            {
                throw new Exception("Invalid role specified");
            }
            _personRepository.Create(new Person(user.Id, account.Name, account.Surname, account.Email));
        } catch (Exception ex) {
            throw new Exception("Error creating user or person", ex);
        }
    }
    public void BlockUser(string username)
    {
        var user = _userRepository.GetActiveByName(username);
        if (user == null)
        {
            throw new Exception("User not found or already inactive");
        }
        user.IsActive = false;
        _userRepository.Update(user);
    }
    public void UnblockUser(string username)
    {
        var user = _userRepository.GetUserByUsername(username);
        if (user == null)
        {
            throw new Exception("User not found or already inactive");
        }
        user.IsActive = true;
        _userRepository.Update(user);
    }
    public AdminUserInfoDto GetUserInfoByName(string username)
    {
        var user = _userRepository.GetActiveByName(username);
        if (user == null)
        {
            throw new Exception("User not found or inactive");
        }
        return _mapper.Map<AdminUserInfoDto>(user);
    }
    public PagedResult<AdminUserInfoDto> GetUsers(int pageNumber, int pageSize)
    {
        var result = _userRepository.GetPaged(pageNumber, pageSize);

        var items = result.Results.Select(_mapper.Map<AdminUserInfoDto>).ToList();
        return new PagedResult<AdminUserInfoDto>(items, result.TotalCount);
    }
}
