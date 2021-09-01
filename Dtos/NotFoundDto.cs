using Swashbuckle.AspNetCore.Annotations;

namespace Catalog.Dtos
{
    public record NotFoundDto
    {
        /// <example>https://tools.ietf.org/html/rfc7231#section-6.5.4</example>
        public string type { get; set; }
        /// <example>Not Found</example>
        public string title { get; set; }
        /// <example>404</example>
        public int status { get; set; }
        /// <example>00-5c77a23f7d13e047b674a25ed06da971-2b17ead2d3540644-00</example>
        public string traceId { get; set; }
    }
}