using System;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Entities
{
    public record User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
    }
}