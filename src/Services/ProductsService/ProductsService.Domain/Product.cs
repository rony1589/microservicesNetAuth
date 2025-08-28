namespace ProductsService.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public string Category { get; set; } = default!;
        public Boolean Status { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
