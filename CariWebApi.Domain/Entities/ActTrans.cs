namespace CariWebApi.Domain.Entities;

public class ActTrans
{
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public Company? Company { get; set; }

    public int AccountId { get; set; }
    public Account? Account { get; set; }

    public int ReceiptId { get; set; }
    public Receipt? Receipt { get; set; }

    public decimal Debit { get; set; } = 0;
    public decimal Credit { get; set; } = 0;

    public DateTime TransDate { get; set; } = DateTime.UtcNow;
}