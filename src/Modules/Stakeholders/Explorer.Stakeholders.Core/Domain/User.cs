using Explorer.BuildingBlocks.Core.Domain;
using System.Text.RegularExpressions;

namespace Explorer.Stakeholders.Core.Domain;

public class User : Entity
{
    private static Regex emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string Email { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; set; }

    public User(string username, string password, string email, UserRole role, bool isActive)
    {
        Username = username;
        Password = password;
        Email = email;
        Role = role;
        IsActive = isActive;
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Username)) throw new ArgumentException("Invalid Name");
        if (string.IsNullOrWhiteSpace(Password)) throw new ArgumentException("Invalid Surname");
        if (!emailRegex.IsMatch(Email)) throw new ArgumentException("Invalid Email");
    }

    public string GetPrimaryRoleName()
    {
        return Role.ToString().ToLower();
    }
}
public enum UserRole
{
    Administrator,
    Author,
    Tourist
}
