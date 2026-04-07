namespace X.Application.Modules.Auth.LogIn;

public class UserLogInCommand(string email, string password)
{
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
}