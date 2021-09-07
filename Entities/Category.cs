using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Entities
{
    public record Category
    {
        [Key]
        public Guid Id { get; set; }
        [Required]

        public string Name { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
        public List<Item> Items { get; set; }

    }
}