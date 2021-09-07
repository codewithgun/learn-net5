using System.ComponentModel.DataAnnotations;

namespace Catalog.Dtos
{
    public record UpdateCategoryDto
    {
        [Required]
        public string Name { get; init; }
    }
}