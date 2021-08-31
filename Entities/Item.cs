using System;
namespace Catalog.Entities
{
    // "record" = RecordTypesd
    // value-based equality instead of memory-reference based equality

    public record Item {
        // { private set; } = immutable
        // init = Can initialize the value, but after that the property is immutable
        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}