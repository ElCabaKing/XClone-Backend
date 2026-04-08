using X.Application.Interfaces;
using X.Domain.Exceptions;
using X.Domain.Interfaces.Repository;
using X.Shared.Helpers;
using X.Shared.Responses;

namespace X.Application.Modules.Auth.LogIn;
public class UserLogInHandler(IToken tokenService, 
IPasswordHash passwordHash,
IUserRepository userRepository)
{
    public async Task<GenericResponse<UserLogInResponse>> Execute(UserLogInCommand command)
    {
        var user = await userRepository.GetUserByEmailAsync(command.Email);
        if (user == null)
        {
            throw new BadRequestException("User or password is incorrect");
        }

        if (!passwordHash.VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new BadRequestException("User or password is incorrect");
        }
        return ResponseHelper.Create(new UserLogInResponse(
            tokenService.GenerateToken(user.Id.ToString()),
            tokenService.GenerateToken(user.Id.ToString())
        ));;
    }
}