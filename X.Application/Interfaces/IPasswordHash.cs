using System;

namespace X.Application.Interfaces;

public interface IPasswordHash
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}