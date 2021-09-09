using Catalog.Dtos;
using Catalog.Manager;
using Catalog.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Catalog.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsersRepository usersRepository;
        private readonly JsonWebTokenManager jsonWebTokenManager;
        public AuthController(IUsersRepository usersRepository, JsonWebTokenManager jsonWebTokenManager)
        {
            this.usersRepository = usersRepository;
            this.jsonWebTokenManager = jsonWebTokenManager;
        }

        [HttpPost("/admin")]
        [SwaggerOperation(
            Summary = "Admin login",
            Description = "Allow admin to login and get jwt token",
            OperationId = "AdminLogin"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Return jwt token")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid credential")]
        public ActionResult<string> AdminLogin(AuthDto authDto)
        {
            if (authDto.Username == "admin" && authDto.Password == "admin")
            {
                return this.jsonWebTokenManager.CreateAdminToken();
            }
            return Unauthorized();
        }

        [HttpPost("/user")]
        [SwaggerOperation(
            Summary = "User login",
            Description = "Allow user to login and get jwt token",
            OperationId = "UserLogin"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Return jwt token")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid credential")]
        public async Task<ActionResult<string>> UserLogin(AuthDto authDto)
        {
            var user = await this.usersRepository.GetAuthUserAsync(authDto);
            if (user is null)
            {
                return Unauthorized();
            }
            return this.jsonWebTokenManager.CreateUserToken(user);
        }
    }
}