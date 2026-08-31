namespace CariWebApi.Application.DTOs;

public class CreateCompanyDto
{
    public string Name { get; set; } = string.Empty;
    public string? TaxNumber { get; set; }
}