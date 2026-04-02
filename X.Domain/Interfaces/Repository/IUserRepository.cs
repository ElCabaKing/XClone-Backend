using System;
using X.Domain.Entities;

namespace X.Domain.Interfaces.Repository;

public interface IUserRepository
{
    public Task<UserEntity> GetUserByIdAsync(Guid id);
    public Task<UserEntity> CreateUserAsync(UserEntity user);
}
