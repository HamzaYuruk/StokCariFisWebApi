using AutoMapper;
using CariWebApi.Application.DTOs;
using CariWebApi.Application.Interfaces;
using CariWebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CariWebApi.Application.Constants;
using Microsoft.Extensions.Logging;

namespace CariWebApi.Application.Services;

public class StockService : IStockService
{
    private readonly IRepository<Stock> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<StockService> _logger;
    private readonly ICurrentUserService _currentUser;

    public StockService(
        IRepository<Stock> repository,
        IMapper mapper,
        ILogger<StockService> logger,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _currentUser = currentUser;
    }

    // stokları getirme 
    public async Task<List<StockDto>> GetAllAsync(string? search, int page, int pageSize)
    {
        IQueryable<Stock> query = _repository.Query()
            .Include(s => s.Company)
            .Where(s => !s.IsDeleted && s.CompanyId == _currentUser.CompanyId);
        
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(s => s.Name.Contains(search) || s.Code.Contains(search));
        }

        var stocks = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return _mapper.Map<List<StockDto>>(stocks);
    }

    // id'ye göre stok getirme
    public async Task<StockDto?> GetByIdAsync(int id)
    {
        var stock = await _repository.Query()
            .Include(s => s.Company)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted && s.CompanyId == _currentUser.CompanyId);

        if (stock == null)
        {
            return null;
        }
        return _mapper.Map<StockDto>(stock);
    }

    // stok oluşturma
    public async Task<StockDto> CreateAsync(CreateStockDto dto)
    {
        if (_currentUser.CompanyId == null)
        {
            throw new ValidationException(ErrorMessages.NoCompanySelected);
        }

        ValidateStockFields(dto.Name, dto.Code, dto.UnitPrice);

        var stock = _mapper.Map<Stock>(dto);
        stock.CompanyId = _currentUser.CompanyId!.Value;

        await _repository.AddAsync(stock);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Yeni stok oluşturuldu: {StockId} - {StockName}", stock.Id, stock.Name);
        
        return _mapper.Map<StockDto>(stock);
    }
    
    // stok update
    public async Task<StockDto?> UpdateAsync(int id, UpdateStockDto dto)
    {
        ValidateStockFields(dto.Name, dto.Code, dto.UnitPrice);

        var stock = await _repository.Query()
            .FirstOrDefaultAsync(s => s.Id == id && s.CompanyId == _currentUser.CompanyId);
        
        if (stock == null)
        {
            _logger.LogWarning("Güncellenmek istenen stok bulunamadı: {StockId}", id);
            return null;
        }

        _mapper.Map(dto, stock);

        _repository.Update(stock);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Stok güncellendi: {StockId} - {StockName}", stock.Id, stock.Name);
        
        return _mapper.Map<StockDto>(stock);
    }

    // stok silme
    public async Task<bool> DeleteAsync(int id)
    {
        var stock = await _repository.Query()
            .FirstOrDefaultAsync(s => s.Id == id && s.CompanyId == _currentUser.CompanyId);
        
        if (stock == null)
        {
            _logger.LogWarning("Silinmek istenen stok bulunamadı: {StockId}", id);
            return false;
        }
        
        stock.IsDeleted = true;
        _repository.Update(stock);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Stok silindi: {StockId} - {StockName}", stock.Id, stock.Name);
        
        return true;
    }

    // tekrar eden kontroller
    private static void ValidateStockFields(string name, string code, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException(ErrorMessages.NameRequired);
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ValidationException(ErrorMessages.CodeRequired);
        }

        if (unitPrice < 0)
        {
            throw new ValidationException(ErrorMessages.NegativePrice);
        }
    }
}