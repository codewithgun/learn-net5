using System;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Entities
{
    // "record" = RecordTypesd
    // value-based equality instead of memory-reference based equality

    public abstract record Item
    {
        // { private set; } = immutable
        // init = Can initialize the value, but after that the property is immutable
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}

namespace Catalog.Entities.Postgres
{
    public record Item : Catalog.Entities.Item
    {
        [Key]
        public new Guid Id { get; set; }

        [Required]
        public new string Name { get; set; }

        [Required]
        public decimal Price { get; set; } = 0;

        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    }
}

namespace Catalog.Entities.Mongo
{
    public record Item : Catalog.Entities.Item { }
}