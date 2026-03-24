using IndianArtVilla.Core.Enums;
using IndianArtVilla.Core.Exceptions;

namespace IndianArtVilla.Core.Entities;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal SubTotal { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CouponCode { get; set; }
    public string? Notes { get; set; }
    public bool IsGiftOrder { get; set; }
    public string? GiftMessage { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public int ShippingAddressId { get; set; }
    public Address ShippingAddress { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? TrackingNumber { get; set; }
    public string? TrackingUrl { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();

    public void Cancel(string note = "Cancelled by customer")
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.Confirmed)
            throw new BusinessRuleViolationException("Order can only be cancelled when it is Pending or Confirmed.");

        Status = OrderStatus.Cancelled;
        AddStatusHistory(OrderStatus.Cancelled, note);
    }

    public void UpdateStatus(OrderStatus newStatus, string? trackingNumber = null, string? trackingUrl = null, string? note = null)
    {
        Status = newStatus;

        if (!string.IsNullOrEmpty(trackingNumber))
            TrackingNumber = trackingNumber;
        if (!string.IsNullOrEmpty(trackingUrl))
            TrackingUrl = trackingUrl;

        if (newStatus == OrderStatus.Shipped)
            ShippedAt = DateTime.UtcNow;
        if (newStatus == OrderStatus.Delivered)
        {
            DeliveredAt = DateTime.UtcNow;
            PaymentStatus = PaymentStatus.Paid;
        }

        AddStatusHistory(newStatus, note);
    }

    private void AddStatusHistory(OrderStatus status, string? note)
    {
        StatusHistory.Add(new OrderStatusHistory
        {
            OrderId = Id,
            Status = status,
            Note = note,
            CreatedAt = DateTime.UtcNow
        });
    }
}
