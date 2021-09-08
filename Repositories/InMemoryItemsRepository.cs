using System;
using System.Linq;
using System.Collections.Generic;
using Catalog.Entities;
using System.Threading.Tasks;

namespace Catalog.Repositories
{
    // Because dependency introduces dependencies. Eg ItemsController depends on InMemoryItemRepository.
    // Therefore, interface used for dependency inversion
    public class InMemoryItemsRepository : IItemsRepository
    {
        private readonly List<Item> items = new() { };

        public async Task UpdateItemAsync(Item item)
        {
            int index = this.items.FindIndex(i => i.Id == item.Id);
            this.items[index] = item;
            await Task.CompletedTask;
        }

        public async Task CreateItemAsync(Item item)
        {
            this.items.Add(item);
            await Task.CompletedTask;
        }

        public async Task DeleteItemAsync(Item item)
        {
            int index = this.items.FindIndex(i => i.Id == item.Id);
            this.items.RemoveAt(index);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await Task.FromResult(items);
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            var items = this.items.Where(item => item.Id == id).SingleOrDefault();
            return await Task.FromResult(items);
        }
    }
}