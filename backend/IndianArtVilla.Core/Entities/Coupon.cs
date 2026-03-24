using IndianArtVilla.Core.Enums;
using IndianArtVilla.Core.Exceptions;

namespace IndianArtVilla.Core.Entities;

public class Coupon
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public CouponType Type { get; set; }
    public decimal Value { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public int? UsageLimit { get; set; }
    public int UsedCount { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;

    public void Validate(decimal orderSubTotal)
    {
        if (ExpiresAt.HasValue && ExpiresAt <= DateTime.UtcNow)
            throw new BusinessRuleViolationException("Coupon has expired.");

        if (UsageLimit.HasValue && UsedCount >= UsageLimit)
            throw new BusinessRuleViolationException("Coupon usage limit reached.");

        if (MinOrderAmount.HasValue && orderSubTotal < MinOrderAmount)
            throw new BusinessRuleViolationException($"Minimum order amount of ₹{MinOrderAmount} required.");
    }

    public decimal CalculateDiscount(decimal subTotal, decimal shippingAmount)
    {
        var discount = Type switch
        {
            CouponType.Percentage => Math.Round(subTotal * Value / 100, 2),
            CouponType.FixedAmount => Value,
            CouponType.FreeShipping => shippingAmount,
            _ => 0
        };

        if (MaxDiscountAmount.HasValue && discount > MaxDiscountAmount.Value)
            discount = MaxDiscountAmount.Value;

        return discount;
    }

    public void IncrementUsage() => UsedCount++;
}
