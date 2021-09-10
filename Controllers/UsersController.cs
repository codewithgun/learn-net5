using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("/register")]
        public async Task<ActionResult<UserDto>> Register(CreateUserDto createUserDto)
        {
            var user = await this.usersRepository.GetUserByEmail(createUserDto.Email);
            if (user != null)
            {
                return BadRequest("Email registered");
            }
            var newUser = new User()
            {
                Email = createUserDto.Email,
                Name = createUserDto.Name,
                Password = createUserDto.Password,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await this.usersRepository.CreateUserAsync(newUser);
            return newUser.AsDto();
        }

        [Authorize(Policy = "Admin")]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get users",
            Description = "Get all users",
            OperationId = "GetUsers"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Return list of users")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden access")]
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await this.usersRepository.GetUsersAsync();
            return users.Select(u => u.AsDto());
        }


    }
}