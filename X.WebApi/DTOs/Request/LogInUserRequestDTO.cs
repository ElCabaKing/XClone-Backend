namespace X.WebApi.DTOs.Request;

public class UserLogInRequestDTO
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}