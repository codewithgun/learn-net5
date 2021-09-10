using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Dtos;
using Catalog.Entities;

namespace Catalog.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserAsync(Guid id);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(User user);

        Task<User> GetUserByEmail(string email);
        Task<User> GetAuthUserAsync(AuthDto autoDto);
    }
}
