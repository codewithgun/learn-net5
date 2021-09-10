using System.ComponentModel.DataAnnotations;

namespace Catalog.Dtos
{
    public record CreateUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }
    }

}