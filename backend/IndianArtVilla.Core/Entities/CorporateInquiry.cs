namespace IndianArtVilla.Core.Entities;

public class CorporateInquiry
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public int EstimatedQuantity { get; set; }
    public DateTime? RequiredByDate { get; set; }
    public string Status { get; set; } = "New";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
