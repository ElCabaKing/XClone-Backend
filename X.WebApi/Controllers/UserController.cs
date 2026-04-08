using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.Application.Modules.User.CreateUser;
using X.WebApi.DTOs.Request;

namespace X.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly CreateUserHandler _createUserHandler;
    

        public UserController(CreateUserHandler createUserHandler)
        {
            _createUserHandler = createUserHandler;
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

        
    }
}
