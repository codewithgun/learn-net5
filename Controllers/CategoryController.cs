using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get categories",
            Description = "Get all categories",
            OperationId = "GetCategories"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Return list of categories")]
        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var items = await this.categoryRepository.GetCategoriesAsync();
            return items.Select(c => c.AsDto());
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get a single category",
            Description = "Get category",
            OperationId = "GetCategory"
        )]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Category not found")]
        [SwaggerResponse(StatusCodes.Status200OK, "Return an single category", typeof(CategoryDto))]
        public async Task<IActionResult> GetCategoryAsync(Guid id)
        {
            var category = await this.categoryRepository.GetCategoryAsync(id);
            if (category is null)
            {
                return NotFound();
            }
            return Ok(category.AsDto());
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new category",
            Description = "Create a new category",
            OperationId = "CreateCategory"
        )]
        public async Task<ActionResult<CategoryDto>> CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            Category category = new()
            {
                Id = Guid.NewGuid(),
                Name = categoryDto.Name,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await this.categoryRepository.CreateCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategoryAsync), new { id = category.Id }, category.AsDto());
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update a category",
            Description = "Update a category",
            OperationId = "UpdateCategory"
        )]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Category not found", typeof(NotFoundDto))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Category updated")]
        public async Task<IActionResult> UpdateCategoryAsync(Guid id, UpdateCategoryDto categoryDto)
        {
            var category = await this.categoryRepository.GetCategoryAsync(id);
            if (category is null)
            {
                return NotFound();
            }
            Category updatedCategory = category with
            {
                Name = categoryDto.Name
            };
            await this.categoryRepository.UpdateCategoryAsync(updatedCategory);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a category",
            Description = "Create a category",
            OperationId = "DeleteCategory"
        )]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Category not found", typeof(NotFoundDto))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Category deleted")]
        public async Task<IActionResult> DeleteCategoryAsync(Guid id)
        {
            var category = await this.categoryRepository.GetCategoryAsync(id);
            if (category is null)
            {
                return NotFound();
            }
            await this.categoryRepository.DeleteItemAsync(category);
            return NoContent();
        }
    }
}