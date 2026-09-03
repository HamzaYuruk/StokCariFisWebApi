namespace CariWebApi.Application.DTOs;

public class MyAccountDto
{
    public int Id { get; set; }
    
    public string Code { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public decimal Balance { get; set; }
    
    public List<ReceiptDto> Receipts { get; set; } = new();
}