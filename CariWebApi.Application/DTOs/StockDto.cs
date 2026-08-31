namespace CariWebApi.Application.DTOs;

public class StockDto
{
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }
    public decimal Balance { get; set; } = 0;
    
}