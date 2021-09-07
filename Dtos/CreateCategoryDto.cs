using System.ComponentModel.DataAnnotations;

namespace Catalog.Dtos
{
    public record CreateCategoryDto
    {
        [Required]
        public string Name { get; init; }
    }
}