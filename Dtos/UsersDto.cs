using System;

namespace Catalog.Dtos
{
    public record UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}