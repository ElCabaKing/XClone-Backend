using System;
using Microsoft.EntityFrameworkCore;
using UserDomain = X.Domain.Entities.User;
using X.Domain.Interfaces.Repository;
using X.Infrastructure.Database.SqlServer.Context;
using X.Infrastructure.Persistence;

namespace X.Infrastructure.Repository;

public class UserRepository(XDbContext context) : IUserRepository
{
    public async Task<UserDomain> GetUserByIdAsync(Guid id)
    {
        var user = await context.Users.FindAsync(id);
        if(user == null)
        {
            throw new Exception("User not found");
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
}
