namespace Test_Api.Models
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
    public enum TransactionType
    {
        visa,
        masterCard,
        cash
    }
    public class Order
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string ApplicationUserId { get; set; }
        public ApplicationUser applicationUser { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public TransactionType TransactionType { get; set; } = TransactionType.visa;
        public string? sessionId { get; set; }
        public string? TransactionId { get; set; }
        public string? carrierId { get; set; }
        public string? carrierName { get; set; }
        public string? TrackingNumber { get; set; }
        public ShippingStatus ShippingStatus { get; set; } = ShippingStatus.Created;
        public DateTime shippodDate { get; set; }

    }
    public enum ShippingStatus
    {
        Created,
        Shipped,
        InTransit,
        Delivered,
        Cancelled
    }
}