
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Entities;
using MongoDB.Driver;

namespace Catalog.Repositories
{
    public class MongoCategoryRepository : ICategoryRepository
    {
        private const string databaseName = "catalog";
        private const string categoryCollectionName = "categories";
        private readonly IMongoCollection<Category> categoryCollection;
        private readonly FilterDefinitionBuilder<Category> filterBuilder = Builders<Category>.Filter;

        public MongoCategoryRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            this.categoryCollection = database.GetCollection<Category>(categoryCollectionName);
        }

        public async Task CreateCategoryAsync(Category category)
        {
            await this.categoryCollection.InsertOneAsync(category);
        }

        public async Task DeleteItemAsync(Category category)
        {
            var filter = filterBuilder.Eq(c => c.Id, category.Id);
            await this.categoryCollection.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await this.categoryCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Category> GetCategoryAsync(Guid id)
        {
            var filter = filterBuilder.Eq(c => c.Id, id);
            return await this.categoryCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            var filter = filterBuilder.Eq(c => c.Id, category.Id);
            await this.categoryCollection.ReplaceOneAsync(filter, category);
        }
    }
}