using System;
using UserDomain = X.Domain.Entities.User;

namespace X.Domain.Interfaces.Repository;

public interface IUserRepository
{
    public Task<UserDomain> GetUserByIdAsync(Guid id);
    public Task<UserDomain> GetUserByEmailAsync(string email);
    public Task<UserDomain> CreateUserAsync(UserDomain user);
}
