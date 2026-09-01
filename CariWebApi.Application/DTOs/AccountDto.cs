namespace CariWebApi.Application.DTOs;

public class AccountDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? TaxNumber { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
}