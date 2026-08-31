namespace CariWebApi.Domain.Entities;

public class ReceiptDetail
{
    public int Id { get; set; }

    public int ReceiptId { get; set; }
    public Receipt? Receipt { get; set; }

    public int StockId { get; set; }
    public Stock? Stock { get; set; }

    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}