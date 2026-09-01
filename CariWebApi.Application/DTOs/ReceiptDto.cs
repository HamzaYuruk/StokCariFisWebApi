namespace CariWebApi.Application.DTOs;

public class ReceiptDto
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string ReceiptNumber { get; set; } = string.Empty;
    public string ReceiptType { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<ReceiptDetailDto> Details { get; set; } = new();
}