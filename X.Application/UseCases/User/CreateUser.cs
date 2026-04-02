namespace X.Application.UseCases.User;

using X.Application.DTOs.Command;
using X.Application.DTOs.Response;
using X.Domain.Entities;
using X.Infrastructure.Repository;

public class CreateUser(UserRepository userRepository)
{

    public Task<int> Execute(CreateUserCommand command)
    {
        return Task.FromResult(0);
    }
}