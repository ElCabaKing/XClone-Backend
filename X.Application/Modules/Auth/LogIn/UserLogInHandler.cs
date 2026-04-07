using X.Application.Interfaces;
using X.Domain.Interfaces.Repository;

namespace X.Application.Modules.Auth.LogIn;
public class UserLogInHandler(IToken tokenService, 
IPasswordHash passwordHash,
IUserRepository userRepository)
{
    public async Task<string> Execute(UserLogInCommand command)
    {
        var user = await userRepository.GetUserByEmailAsync(command.Email);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (!passwordHash.VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new Exception("Invalid password");
        }

        var token = tokenService.GenerateToken(user.Id.ToString(), user.Email);
        return token;
    }
}