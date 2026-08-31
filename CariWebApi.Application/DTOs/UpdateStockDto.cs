namespace CariWebApi.Application.DTOs;

public class UpdateStockDto
{
    public string Code { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public string Unit { get; set; } = string.Empty;
    
    public decimal UnitPrice { get; set; }
}