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

namespace Catalog.Controllers
{
    [ApiController]
    // [Route("[controller]")] // Automatically get class name as route. Eg: GET /items
    [Route("items")] // Manual specify route name
    public class ItemsController : ControllerBase
    {
        // It can be in-memory repository, mongodb, postgresql
        // Thanks to dependency inversion
        private readonly IItemsRepository repository;

        // constructor
        // Controller is instantiate everytime when there is a request
        public ItemsController(IItemsRepository repository)
        {
            this.repository = repository;
        }

        // GET /items
        [HttpGet]
        // Instead of returning Item, which is a database entity,
        // We return DTO because it serve as a contract with the client.
        // And, breaking the DTO is a no-go
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            // Conversion of Item to ItemDto
            var items = await this.repository.GetItemsAsync();
            return items.Select(item => item.AsDto());
        }

        // GET /items/{id}
        [HttpGet("{id}")]
        // ActionResult allow us to return more than one type
        // ActionResult = HTTP result, such as OkResponse, NotFoundResponse
        // which are, results of action method (controller)
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemDto))]
        public async Task<IActionResult> GetItem(Guid id)
        {
            Item item = await this.repository.GetItemAsync(id);
            if (item is null)
            {
                return NotFound();
            }
            return Ok(item.AsDto());
        }

        // POST /items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItem(CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await this.repository.CreateItemAsync(item);
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }

        // PUT /items
        /// <summary>
        /// Testing
        /// </summar>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            Item existingItem = await this.repository.GetItemAsync(id);
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

            await this.repository.UpdateItemAsync(updatedItem);
            return NoContent();
        }

        // DELETE /items
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteItem(Guid id)
        {
            Item existingItem = await this.repository.GetItemAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }
            await this.repository.DeleteItemAsync(existingItem);
            return NoContent();
        }
    }
}