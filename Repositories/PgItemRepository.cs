using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Context;
using Catalog.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Repositories
{
    public class PgItemRepository : IItemsRepository
    {
        private readonly PostgresDbContext _context;
        public PgItemRepository(PostgresDbContext context)
        {
            this._context = context;
        }

        private IQueryable<Item> _GetItemById(Guid id)
        {
            return this._context.Items.AsNoTracking().Where(i => i.Id == id);
        }

        public async Task CreateItemAsync(Item item)
        {
            this._context.AddAsync(item);
            await this._context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(Item item)
        {
            this._context.Remove(item);
            await this._context.SaveChangesAsync();
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            return await this._GetItemById(id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await this._context.Items.ToListAsync();
        }

        public async Task UpdateItemAsync(Item item)
        {
            var existingItem = await this._GetItemById(item.Id).SingleOrDefaultAsync();
            existingItem.Name = item.Name;
            existingItem.Price = item.Price;
            this._context.Entry(existingItem).State = EntityState.Modified;
            await this._context.SaveChangesAsync();
        }
    }
}