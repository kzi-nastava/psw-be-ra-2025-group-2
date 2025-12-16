using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly IPersonRepository _personRepository;

    public AuthenticationService(IUserRepository userRepository, IPersonRepository personRepository, ITokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
        _personRepository = personRepository;
    }

    public AuthenticationTokensDto Login(CredentialsDto credentials)
    {
        var user = _userRepository.GetActiveByName(credentials.Username);
        if (user == null || credentials.Password != user.Password)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        long personId;
        try
        {
            personId = _userRepository.GetPersonId(user.Id);
        }
        catch (KeyNotFoundException)
        {
            personId = 0;
        }
        return _tokenGenerator.GenerateAccessToken(user, personId);
    }

    public AuthenticationTokensDto RegisterTourist(AccountRegistrationDto account)
    {
        if(_userRepository.Exists(account.Username))
            throw new EntityValidationException("Provided username already exists.");

        // Prvo vidi da li domenski objekti mogu da se naprave sa ovim podacima, umesto da se kreira user, a da person pukne zbog nevazeceg Email
        var user = new User(account.Username, account.Password, UserRole.Tourist, true);
        var person = new Person(-1, account.Name, account.Surname, account.Email);

        user = _userRepository.Create(new User(user.Username, user.Password, UserRole.Tourist, true));
        person = _personRepository.Create(new Person(user.Id, person.Name, person.Surname, person.Email));

        return _tokenGenerator.GenerateAccessToken(user, person.Id);
    }
}