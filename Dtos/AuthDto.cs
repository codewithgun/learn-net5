using System.ComponentModel.DataAnnotations;

namespace Catalog.Dtos
{
    public record AuthDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}