namespace CariWebApi.Domain.Entities;

public class StockTrans
{
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public Company? Company { get; set; }

    public int StockId { get; set; }
    public Stock? Stock { get; set; }

    public int ReceiptId { get; set; }
    public Receipt? Receipt { get; set; }

    public decimal Quantity { get; set; }
    public DateTime TransDate { get; set; } = DateTime.UtcNow;
}