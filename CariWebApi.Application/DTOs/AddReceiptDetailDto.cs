namespace CariWebApi.Application.DTOs;

public class AddReceiptDetailDto
{
    public int StockId { get; set; }
    
    public decimal Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
}