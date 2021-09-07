
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Context;
using Catalog.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Repositories
{
    public class PgCategoryRepository : ICategoryRepository
    {
        private readonly PostgresDbContext _context;
        public PgCategoryRepository(PostgresDbContext context)
        {
            this._context = context;
        }
        public async Task CreateCategoryAsync(Category category)
        {
            await this._context.Categories.AddAsync(category);
            await this._context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(Category category)
        {
            this._context.Categories.Remove(category);
            await this._context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await this._context.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<Category> GetCategoryAsync(Guid Id)
        {
            return await this._context.Categories.AsNoTracking().Where(c => c.Id == Id).FirstOrDefaultAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            var existingCategory = await this._context.Categories.Where(c => c.Id == category.Id).FirstOrDefaultAsync();
            existingCategory.Name = category.Name;
            await this._context.SaveChangesAsync();
        }
    }
}