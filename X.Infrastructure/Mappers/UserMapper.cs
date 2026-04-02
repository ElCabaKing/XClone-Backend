using DomainUser = X.Domain.Entities.User;
using UserStatus = X.Domain.Enums.UserStatusEnum;
using PersistenceUser = X.Infrastructure.Persistence.User;

public class UserMapper
{
    public static DomainUser ToEntity(PersistenceUser user)
    {
        return new DomainUser(
            user.Id,
            user.Username,
            user.Email,
            user.PasswordHash,
            user.CreatedAt,
            user.IsVerified,
            (UserStatus)user.StatusId,
            user.FirstName,
            user.LastName,
            user.ProfilePictureUrl
        );
    }   

    public static PersistenceUser ToDb(DomainUser entity)
    {
        return new PersistenceUser
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            PasswordHash = entity.PasswordHash,
            CreatedAt = entity.CreatedAt,
            IsVerified = entity.IsVerified,
            StatusId = (byte)entity.StatusId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            ProfilePictureUrl = entity.ProfilePictureUrl
        };
    }
}