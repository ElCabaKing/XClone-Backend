namespace X.Application.Modules.User.CreateUser;

using X.Application.Interfaces;
using X.Domain.Entities;
using X.Domain.Interfaces.Repository;
using X.Shared.Helpers;
using X.Shared.Responses;

public class CreateUserHandler(IUserRepository userRepository, IPasswordHash passwordHash,
IStorage storage)
{

    public async Task<GenericResponse<User>> Execute(CreateUserCommand command)
    {
        var user = new User(
            Guid.NewGuid(),
            command.Username,
            command.Email,
            passwordHash.HashPassword(command.Password),
            DateTime.UtcNow,
            false,
            Domain.Enums.UserStatusEnum.active,
            command.FirstName,
            command.LastName,
            null
        );

        if (command.ProfilePicture != null)
        {
            var profilePictureUrl = storage.UploadFileAsync(command.ProfilePicture, command.ProfilePictureFileName ?? $"{user.Id}_profile_picture", command.ProfilePictureContentType ?? "image/jpeg").Result;
            user.ProfilePictureUrl = profilePictureUrl;
        }

        var createdUser = userRepository.CreateUserAsync(user).Result;

        return ResponseHelper.Create(createdUser);
    }
}