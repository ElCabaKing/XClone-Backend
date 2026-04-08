using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.Application.Modules.User.CreateUser;
using X.WebApi.DTOs.Request;

namespace X.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController (
        CreateUserHandler createUserHandler
        ): ControllerBase
    {
        private readonly CreateUserHandler _createUserHandler = createUserHandler;

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
            return Ok(await _createUserHandler.Execute(createUserCommand));
        }

        
    }
}
