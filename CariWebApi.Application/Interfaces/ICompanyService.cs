using CariWebApi.Application.DTOs;
using CariWebApi.Application.DTOs.Auth;

namespace CariWebApi.Application.Interfaces;

public interface ICompanyService
{
    Task<List<CompanyDto>> GetAllAsync(int userId);
    
    Task<CompanyDto?> GetByIdAsync(int id);
    
    Task<CreateCompanyResponseDto> CreateAsync(CreateCompanyDto dto,int userId);
    
    Task<CompanyDto?> UpdateAsync(int id, UpdateCompanyDto dto);
    
    Task<bool> DeleteAsync(int id);
    
}