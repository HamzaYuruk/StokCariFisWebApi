using CariWebApi.Application.DTOs;

namespace CariWebApi.Application.Interfaces;

public interface IReceiptService
{
    Task<List<ReceiptDto>> GetAllAsync(int page, int pageSize);
    
    Task<ReceiptDto?> GetByIdAsync(int id);
    
    Task<ReceiptDto> CreateAsync(CreateReceiptDto dto);
    
    Task<ReceiptDto?> AddDetailAsync(int receiptId, AddReceiptDetailDto dto);
    
    Task<ReceiptDto?> UpdateAsync(int id, CreateReceiptDto dto);
    
    Task<bool> DeleteAsync(int id);
    
    Task<ReceiptDto?> ApproveAsync(int id);
}