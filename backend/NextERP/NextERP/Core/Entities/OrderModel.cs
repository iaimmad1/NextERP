namespace NextERP.Core.Entities
{
    public class OrderModel
    {
        public int OrderId { get; set; }
        public int TenantId { get; set; }
        public virtual TenantModel? Tenant { get; set; }

        public int? CustomerId { get; set; }        // null = guest checkout
        public virtual UserModel? Customer { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty; // COD, Card
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual ICollection<OrderItemModel> Items { get; set; } = new List<OrderItemModel>();
    }

    public class OrderItemModel
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public virtual OrderModel? Order { get; set; }

        public int ProductId { get; set; }
        public virtual ProductModel? Product { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }
}
