namespace NextERP.Core.Entities
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public int TenantId { get; set; }
        public virtual TenantModel? Tenant { get; set; }

        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }

        // Navigation
        public virtual ICollection<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();
    }
}
