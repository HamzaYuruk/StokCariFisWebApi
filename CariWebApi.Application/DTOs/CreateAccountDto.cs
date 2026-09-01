namespace CariWebApi.Application.DTOs;

public class CreateAccountDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? TaxNumber { get; set; }
}