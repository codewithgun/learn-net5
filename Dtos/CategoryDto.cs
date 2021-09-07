using System;

namespace Catalog.Dtos
{
    public record CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}