namespace X.Application.Modules.User.CreateUser;

using X.Application.Interfaces;
using X.Domain.Entities;
using X.Domain.Interfaces.Repository;

public class CreateUserHandler(IUserRepository userRepository, IPasswordHash passwordHash,
IStorage storage)
{

    public Task<int> Execute(CreateUserCommand command)
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

        return Task.FromResult(1);
    }
}