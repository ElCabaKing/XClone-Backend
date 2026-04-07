using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using X.Application.Modules.Auth.LogIn;
using X.Application.Modules.User.CreateUser;
using X.WebApi.DTOs.Request;

namespace X.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CreateUserHandler _createUserHandler;
        private readonly UserLogInHandler _logInHandler;

        public UserController(CreateUserHandler createUserHandler,
         UserLogInHandler logInHandler)
        {
            _createUserHandler = createUserHandler;
            _logInHandler = logInHandler;
        }

        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserRequestDTO createUserDTO)
        {
            var createUserCommand = new CreateUserCommand(
                createUserDTO.Username,
                createUserDTO.Email,
                createUserDTO.Password,
                createUserDTO.FirstName,
                createUserDTO.LastName,
                createUserDTO.ProfilePicture?.OpenReadStream(),
                createUserDTO.ProfilePicture?.FileName,
                createUserDTO.ProfilePicture?.ContentType
            );

            var result = await _createUserHandler.Execute(createUserCommand);
            return Ok(new { message = "User created successfully", result });
        }

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
