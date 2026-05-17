namespace NextERP.Features.Orders.DTOs
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class CreateOrderRequestDto
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public List<CreateOrderItemRequestDto> Items { get; set; } = new();
    }

    public class CreateOrderItemRequestDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
