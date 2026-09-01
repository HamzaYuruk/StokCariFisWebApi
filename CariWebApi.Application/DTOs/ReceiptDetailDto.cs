namespace CariWebApi.Application.DTOs;

public class ReceiptDetailDto
{
    public int Id { get; set; }
    
    public int StockId { get; set; }
    
    public string StockName { get; set; } = string.Empty;
    
    public decimal Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal LineTotal { get; set; }
}