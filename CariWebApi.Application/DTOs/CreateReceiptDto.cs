namespace CariWebApi.Application.DTOs;

public class CreateReceiptDto
{
    public int AccountId { get; set; }
    
    public string ReceiptType { get; set; } = string.Empty;
    
    public DateTime Date { get; set; }
}