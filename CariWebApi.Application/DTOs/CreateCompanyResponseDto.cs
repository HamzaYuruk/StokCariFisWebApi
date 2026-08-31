namespace CariWebApi.Application.DTOs.Auth;

public class CreateCompanyResponseDto
{
    public CompanyDto Company { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
}