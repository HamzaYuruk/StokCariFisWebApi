using CariWebApi.Application.DTOs;

namespace CariWebApi.Application.Interfaces;

public interface IStockService
{
    Task<List<StockDto>> GetAllAsync(string? search, int page, int pageSize);
    Task<StockDto?> GetByIdAsync(int id);
    Task<StockDto> CreateAsync(CreateStockDto dto);
    
    Task<StockDto?> UpdateAsync(int id, UpdateStockDto dto);
    
    Task<bool> DeleteAsync(int id);
    
}