using System;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Entities
{
    // "record" = RecordTypesd
    // value-based equality instead of memory-reference based equality

    public record Item
    {
        // { private set; } = immutable
        // init = Can initialize the value, but after that the property is immutable
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}

// namespace Catalog.Entities.Postgres
// {
//     public record Item : Catalog.Entities.Item
//     {
//         [Key]
//         public new Guid Id { get; set; }

//         [Required]
//         public new string Name { get; set; }

//         [Required]
//         public new decimal Price { get; set; } = 0;

//         public new DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

//         public Guid CategoryId { get; set; }

//         public Category Category { get; set; }
//     }
// }

// namespace Catalog.Entities.Mongo
// {
//     public record Item : Catalog.Entities.Item { }
// }