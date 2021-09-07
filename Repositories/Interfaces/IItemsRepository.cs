using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Entities;

namespace Catalog.Repositories
{
    public interface IItemsRepository<T> where T : Item
    {
        Task<IEnumerable<T>> GetItemsAsync();
        Task<T> GetItemAsync(Guid id);
        Task CreateItemAsync(T item);
        Task UpdateItemAsync(T item);

        Task DeleteItemAsync(T item);
    }
}
