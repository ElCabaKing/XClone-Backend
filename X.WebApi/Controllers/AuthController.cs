using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using X.Application.Modules.Auth.LogIn;
using X.WebApi.DTOs.Request;

namespace X.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        UserLogInHandler logInHandler
        ) : ControllerBase
    {
        private readonly UserLogInHandler _logInHandler = logInHandler;

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] UserLogInRequestDTO logInDTO)
        {
            var logInCommand = new UserLogInCommand(
                logInDTO.Email,
                logInDTO.Password
            );

            var token = await _logInHandler.Execute(logInCommand);
            return Ok(new { token });
        }
    }
}
