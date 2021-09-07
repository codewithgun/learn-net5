using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Entities;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Repositories
{
    public class MongoItemRepository : IItemsRepository
    {
        private const string databaseName = "catalog";
        private const string collectionName = "items";
        private readonly IMongoCollection<Item> itemsCollection;
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;
        public MongoItemRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            this.itemsCollection = database.GetCollection<Item>(collectionName);
        }
        public async Task CreateItemAsync(Item item)
        {
            await this.itemsCollection.InsertOneAsync(item);
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            // return this.itemsCollection.Find(new BsonDocument()).ToList();
            return await this.itemsCollection.Find(_ => true).ToListAsync(); // Lambda function
        }
        public async Task<Item> GetItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            return await this.itemsCollection.Find(filter).SingleOrDefaultAsync();
        }
        public async Task UpdateItemAsync(Item item)
        {
            var filter = filterBuilder.Eq(item => item.Id, item.Id);
            await this.itemsCollection.ReplaceOneAsync(filter, item);
        }

        public async Task DeleteItemAsync(Item item)
        {
            var filter = filterBuilder.Eq(item => item.Id, item.Id);
            await this.itemsCollection.DeleteOneAsync(filter);
        }
    }
}