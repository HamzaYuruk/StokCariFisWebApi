using CariWebApi.Domain.Enums;

namespace CariWebApi.Domain.Entities;

public class Receipt
{
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public Company? Company { get; set; }

    public int AccountId { get; set; }
    public Account? Account { get; set; }

    public string ReceiptNumber { get; set; } = string.Empty;
    public ReceiptType ReceiptType { get; set; }
    public DateTime Date { get; set; }
    public ReceiptStatus Status { get; set; } = ReceiptStatus.Draft;

    public decimal TotalAmount { get; set; } = 0;

    public bool IsDeleted { get; set; } = false;

    public ICollection<ReceiptDetail> Details { get; set; } = new List<ReceiptDetail>();
}