using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Catalog.Context;
using Catalog.Dtos;
using Catalog.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Repositories
{
    public class PgUsersRepository : IUsersRepository
    {
        private readonly PostgresDbContext context;
        public PgUsersRepository(PostgresDbContext context)
        {
            this.context = context;
        }
        public async Task CreateUserAsync(User user)
        {
            await this.context.Users.AddAsync(user);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            this.context.Users.Remove(user);
            await this.context.SaveChangesAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await this.context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetAuthUserAsync(AuthDto authDto)
        {
            var user = await this.context.Users.Where(u => u.Email == authDto.Username && u.Password == authDto.Password).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            var user = await this.context.Users.AsNoTracking().Where(u => u.Id == id).FirstOrDefaultAsync();
            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = await this.context.Users.AsNoTracking().ToListAsync();
            return users;
        }

        public async Task UpdateUserAsync(User user)
        {
            var toUpdate = await this.context.Users.Where(u => u.Id == user.Id).FirstOrDefaultAsync();
            toUpdate.Name = user.Name;
            if (user.Password != null)
            {
                toUpdate.Password = user.Password;
            }
            await this.context.SaveChangesAsync();
        }
    }
}