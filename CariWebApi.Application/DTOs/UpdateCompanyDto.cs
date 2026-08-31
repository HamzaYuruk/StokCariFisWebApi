namespace CariWebApi.Application.DTOs;

public class UpdateCompanyDto
{
    public string Name { get; set; } = string.Empty;
    public string? TaxNumber { get; set; }
}