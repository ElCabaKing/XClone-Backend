namespace X.Application.Modules.Auth.LogIn;

public class UserLogInResponse(string token, string refreshToken)
{
    public string Token { get; set; } = token;
    public string RefreshToken { get; set; } = refreshToken;

}