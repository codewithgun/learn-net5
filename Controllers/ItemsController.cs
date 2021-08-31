using System;
using Microsoft.AspNetCore.Mvc;
using Catalog.Repositories;
using Catalog.Entities;
using System.Collections.Generic;

namespace Catalog.Controllers {
    [ApiController]
    [Route("[controller]")] // Automatically get class name as route. Eg: GET /items
    // [Route("items")] // Manual specify route name
    public class ItemsController: ControllerBase {
        // It can be in-memory repository, mongodb, postgresql
        // Thanks to dependency inversion
        private readonly IItemsRepository repository;

        // constructor
        // Controller is instantiate everytime when there is a request
        public ItemsController(IItemsRepository repository){
            this.repository = repository;
        }
    
        // GET /items
        [HttpGet]
        public IEnumerable<Item> GetItems() {
            return this.repository.GetItems();
        }

        // GET /items/{id}
        [HttpGet("{id}")]
        public ActionResult<Item> GetItem(Guid id) {
            Item item =  this.repository.GetItem(id);
            if(item is null){
                return NotFound();
            }
            return item;
        }
    }
}