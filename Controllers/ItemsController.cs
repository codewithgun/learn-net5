using System.Net;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

using Catalog.Repositories;
using Catalog.Entities;
using Catalog.Dtos;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Controllers
{
    [ApiController]
    // [Route("[controller]")] // Automatically get class name as route. Eg: GET /items
    [Route("items")] // Manual specify route name
    public class ItemsController : ControllerBase
    {
        // It can be in-memory repository, mongodb, postgresql
        // Thanks to dependency inversion
        private readonly IItemsRepository itemRepository;
        private readonly ICategoryRepository categoryRepository;

        // constructor
        // Controller is instantiate everytime when there is a request
        public ItemsController(IItemsRepository itemRepository, ICategoryRepository categoryRepository)
        {
            this.itemRepository = itemRepository;
            this.categoryRepository = categoryRepository;
        }

        // GET /items
        [HttpGet]
        // Instead of returning Item, which is a database entity,
        // We return DTO because it serve as a contract with the client.
        // And, breaking the DTO is a no-go
        [SwaggerOperation(
            Summary = "Get all items",
            Description = "Get all items",
            OperationId = "GetItems"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Return list of items")]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            // Conversion of Item to ItemDto
            var items = await this.itemRepository.GetItemsAsync();
            return items.Select(item => item.AsDto());
        }

        // GET /items/{id}
        [HttpGet("{id}")]
        // ActionResult allow us to return more than one type
        // ActionResult = HTTP result, such as OkResponse, NotFoundResponse
        // which are, results of action method (controller)
        [SwaggerOperation(
            Summary = "Get an item",
            Description = "Get an item using item id",
            OperationId = "GetItemById"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Return an single item", typeof(ItemDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Item not found", typeof(NotFoundDto))]
        public async Task<IActionResult> GetItemAsync(Guid id)
        {
            Item item = await this.itemRepository.GetItemAsync(id);
            if (item is null)
            {
                return NotFound();
            }
            return Ok(item.AsDto());
        }

        // POST /items
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new item",
            Description = "Create a new item",
            OperationId = "CreateNewItem"
        )]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Category not found", typeof(NotFoundDto))]
        [SwaggerResponse(StatusCodes.Status201Created, "Item has been created", typeof(ItemDto))]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
        {
            var category = await this.categoryRepository.GetCategoryAsync(itemDto.categoryId);
            if (category is null)
            {
                return NotFound();
            }
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow,
                CategoryId = category.Id
            };
            await this.itemRepository.CreateItemAsync(item);
            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto()); // .Net runtime automatically remove Async suffix, check Startup.cs to stop suffix auto removal
        }

        // PUT /items
        [Authorize(Policy = "Admin")]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update item",
            Description = "Update Item",
            OperationId = "UpdateItemById"
        )]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Item to update not found", typeof(NotFoundDto))]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
        {
            Item existingItem = await this.itemRepository.GetItemAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            // With-expression
            // Create a new copy of existingItem with the following properties modifed
            Item updatedItem = existingItem with
            {
                Name = itemDto.Name,
                Price = itemDto.Price
            };

            if (itemDto.categoryId != Guid.Empty)
            {
                Category category = await this.categoryRepository.GetCategoryAsync(itemDto.categoryId);
                if (category is null)
                {
                    return NotFound();
                }
                updatedItem.CategoryId = category.Id;
            }

            await this.itemRepository.UpdateItemAsync(updatedItem);
            return NoContent();
        }

        // DELETE /items
        [Authorize(Policy = "Admin")]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete an item",
            Description = "Delete an item",
            OperationId = "DeleteItemById"
        )]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Item not found", typeof(NotFoundDto))]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteItem(Guid id)
        {
            Item existingItem = await this.itemRepository.GetItemAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }
            await this.itemRepository.DeleteItemAsync(existingItem);
            return NoContent();
        }
    }
}