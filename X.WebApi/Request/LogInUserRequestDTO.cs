using System.ComponentModel.DataAnnotations;
using X.Shared.Constants;

namespace X.WebApi.DTOs.Request;

public class UserLogInRequestDTO
{
    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [EmailAddress(ErrorMessage = ValidationConstants.EMAIL_ADDRESS)]
    public string Email { get; set; } = default!;
    
    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    public string Password { get; set; } = default!;
}