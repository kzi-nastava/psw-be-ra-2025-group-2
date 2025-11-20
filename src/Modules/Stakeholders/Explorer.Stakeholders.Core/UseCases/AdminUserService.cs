using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;
using AutoMapper;

namespace Explorer.Stakeholders.Core.UseCases;
public class AdminUserService
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

    public void CreateUser(AccountRegistrationDto account)
    {
        try {
            var user = _userRepository.Create(new User(account.Username, account.Password, account.Email, UserRole.Tourist, true));
            _personRepository.Create(new Person(user.Id, account.Name, account.Surname, account.Email));
        } catch (Exception ex) {
            throw new Exception("Error creating user and person", ex);
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
        // _userRepository.Update(user);
    }
    public PagedResult<AdminUserInfoDto> GetUsers(int pageNumber, int pageSize)
    {
        // var result = _userRepository.GetPaged(pageNumber, pageSize);

        // var items = result.Results.Select(_mapper.Map<AdminUserInfoDto>).ToList();
        // return new PagedResult<AdminUserInfoDto>(items, result.TotalCount);
        return new PagedResult<AdminUserInfoDto>([], 0);
    }
}
