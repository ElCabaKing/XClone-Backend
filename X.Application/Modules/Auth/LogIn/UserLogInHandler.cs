using X.Application.Interfaces;
using X.Domain.Interfaces.Repository;
using X.Shared.Helpers;
using X.Shared.Responses;

namespace X.Application.Modules.Auth.LogIn;
public class UserLogInHandler(IToken tokenService, 
IPasswordHash passwordHash,
IUserRepository userRepository)
{
    public async Task<GenericResponse<string>> Execute(UserLogInCommand command)
    {
        var user = await userRepository.GetUserByEmailAsync(command.Email);
        if (user == null)
        {
            throw new Exception("User or password is incorrect");
        }

        if (!passwordHash.VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new Exception("User or password is incorrect");
        }

        var token = tokenService.GenerateToken(user.Id.ToString());
        return ResponseHelper.Create(token);
    }
}