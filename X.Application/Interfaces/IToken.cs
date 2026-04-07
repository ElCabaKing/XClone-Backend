namespace X.Application.Interfaces;

public interface IToken
{
    string GenerateToken(string userId, string email);
}