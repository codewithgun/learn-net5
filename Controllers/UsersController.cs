using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository usersRepository;
        public UsersController(IUsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get users",
            Description = "Get all users",
            OperationId = "GetUsers"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Return list of users")]
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await this.usersRepository.GetUsersAsync();
            return users.Select(u => u.AsDto());
        }


    }
}