using CariWebApi.Application.DTOs;

namespace CariWebApi.Application.Interfaces;

public interface IAccountService
{
    Task<List<AccountDto>> GetAllAsync(string? search, int page, int pageSize);
    
    Task<AccountDto?> GetByIdAsync(int id);
    
    Task<AccountDto> CreateAsync(CreateAccountDto dto);
    
    Task<AccountDto?> UpdateAsync(int id, UpdateAccountDto dto);
    
    Task<bool> DeleteAsync(int id);
    
    Task<bool> LinkUserAsync(int accountId, LinkAccountUserDto dto);
    
    Task<MyAccountDto?> GetMyAccountAsync();
}