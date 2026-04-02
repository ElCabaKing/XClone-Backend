namespace X.Application.UseCases.User;

using X.Application.DTOs.Command;
using X.Application.DTOs.Response;
using X.Domain.Entities;
using X.Domain.Interfaces.Repository;

public class CreateUser(IUserRepository userRepository)
{

    public Task<int> Execute(CreateUserCommand command)
    {
        return Task.FromResult(0);
    }
}