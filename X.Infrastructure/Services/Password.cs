using System;
using X.Application.Interfaces;

namespace X.Infrastructure.Services;

public class Password : IPasswordHash
{
    public Password()
    {
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}