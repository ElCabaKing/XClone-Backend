namespace X.WebApi.DTOs.Request;

public class CreateUserRequestDTO
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public IFormFile? ProfilePicture { get; set; }
}