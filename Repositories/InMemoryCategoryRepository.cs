using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Entities;

namespace Catalog.Repositories
{
    public class InMemoryCategoryRepository : ICategoryRepository
    {
        private readonly List<Category> categories = new()
        {
            new Category() { Id = Guid.NewGuid(), Name = "Consumable", CreatedDate = DateTimeOffset.UtcNow }
        };

        public async Task CreateCategoryAsync(Category category)
        {
            this.categories.Add(category);
            await Task.CompletedTask;
        }

        public async Task DeleteItemAsync(Category category)
        {
            Category toDelete = this.categories.Find(c => c.Id == category.Id);
            this.categories.Remove(toDelete);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await Task.FromResult(this.categories);
        }

        public async Task<Category> GetCategoryAsync(Guid id)
        {
            return await Task.FromResult(this.categories.Find(c => c.Id == id));
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            var index = this.categories.FindIndex(c => c.Id == category.Id);
            this.categories[index] = category;
            await Task.CompletedTask;
        }
    }
}