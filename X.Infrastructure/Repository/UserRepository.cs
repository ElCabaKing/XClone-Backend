
using Microsoft.EntityFrameworkCore;
using UserDomain = X.Domain.Entities.User;
using X.Domain.Interfaces.Repository;
using X.Infrastructure.Database.SqlServer.Context;
using X.Infrastructure.Persistence;
using X.Domain.Exceptions;
using X.Shared.Constants;

namespace X.Infrastructure.Repository;

public class UserRepository(XDbContext context) : IUserRepository
{
    public async Task<UserDomain> GetUserByIdAsync(Guid id)
    {
        var user = await context.Users.FindAsync(id);
        if(user == null)
        {
            throw new NotFoundException(ResponseConstants.USER_NOT_FOUND);
        }
        return UserMapper.ToEntity(user);
    }

    public async Task<UserDomain> CreateUserAsync(UserDomain user)
    {
        var userMapped = new User
        {
            Username = user.Username,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            CreatedAt = user.CreatedAt,
            IsVerified = user.IsVerified,
            StatusId = (byte)user.StatusId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
        await context.Users.AddAsync(userMapped);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<UserDomain> GetUserByEmailAsync(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if(user == null)
        {
            throw new NotFoundException(ResponseConstants.USER_NOT_FOUND);
        }
        return UserMapper.ToEntity(user);
    }
}
