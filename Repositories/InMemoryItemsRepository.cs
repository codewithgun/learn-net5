using System;
using System.Linq;
using System.Collections.Generic;
using Catalog.Entities;

namespace Catalog.Repositories {
    public class InMemoryItemsRepository {
        private readonly List<Item> items = new(){
            new Item { Id = Guid.NewGuid(), Name = "Potion", Price = 9, CreatedDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "Iron Sword", Price = 25, CreatedDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "Rounded Shield", Price = 12, CreatedDate = DateTimeOffset.UtcNow }
        };

        public IEnumerable<Item> GetItems() {
            return this.items;
        }

        public Item GetItem(Guid id){
            return this.items.Where(item => item.Id == id).SingleOrDefault();
        }
    }
}