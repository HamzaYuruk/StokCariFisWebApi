using AutoMapper;
using CariWebApi.Application.Constants;
using CariWebApi.Application.DTOs;
using CariWebApi.Application.Constants;
using CariWebApi.Application.Interfaces;
using CariWebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CariWebApi.Application.Services;

public class AccountService : IAccountService
{
    private readonly IRepository<Account> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<AccountService> _logger;
    private readonly ICurrentUserService _currentUser;

    public AccountService(
        IRepository<Account> repository,
        IMapper mapper,
        ILogger<AccountService> logger,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _currentUser = currentUser;
    }

    // cari hesaplarını getirme
    public async Task<List<AccountDto>> GetAllAsync(string? search, int page, int pageSize)
    {
        IQueryable<Account> query = _repository.Query()
            .Where(a => !a.IsDeleted && a.CompanyId == _currentUser.CompanyId);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(a => a.Name.Contains(search) || a.Code.Contains(search));
        }

        var accounts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return _mapper.Map<List<AccountDto>>(accounts);
    }

    // id'si verilen cari hesabını getirme
    public async Task<AccountDto?> GetByIdAsync(int id)
    {
        var account = await _repository.Query()
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted && a.CompanyId == _currentUser.CompanyId);

        if (account == null)
        {
            return null;
        }
        return _mapper.Map<AccountDto>(account);
    }

    // cari hesabı oluşturma
    public async Task<AccountDto> CreateAsync(CreateAccountDto dto)
    {
        // gerekli if kontrolleri
        if (_currentUser.CompanyId == null)
        {
            throw new ValidationException(ErrorMessages.NoCompanySelected);
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ValidationException(ErrorMessages.NameRequired);
        }

        if (string.IsNullOrWhiteSpace(dto.Code))
        {
            throw new ValidationException(ErrorMessages.CodeRequired);
        }

        var account = _mapper.Map<Account>(dto);
        account.CompanyId = _currentUser.CompanyId!.Value;

        await _repository.AddAsync(account);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Yeni cari oluşturuldu: {AccountId} - {AccountName}", account.Id, account.Name);

        return _mapper.Map<AccountDto>(account);
    }

    // cari hesabını güncelleme
    public async Task<AccountDto?> UpdateAsync(int id, UpdateAccountDto dto)
    {
        // gerekli if kontrolleri
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ValidationException(ErrorMessages.NameRequired);
        }

        if (string.IsNullOrWhiteSpace(dto.Code))
        {
            throw new ValidationException(ErrorMessages.CodeRequired);
        }

        var account = await _repository.Query()
            .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == _currentUser.CompanyId);

        if (account == null)
        {
            _logger.LogWarning("Güncellenmek istenen cari bulunamadı: {AccountId}", id);
            return null;
        }

        _mapper.Map(dto, account);

        _repository.Update(account);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Cari güncellendi: {AccountId} - {AccountName}", account.Id, account.Name);

        return _mapper.Map<AccountDto>(account);
    }

    // account delete
    public async Task<bool> DeleteAsync(int id)
    {
        var account = await _repository.Query()
            .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == _currentUser.CompanyId);

        if (account == null)
        {
            _logger.LogWarning("Silinmek istenen cari bulunamadı: {AccountId}", id);
            return false;
        }

        account.IsDeleted = true;
        _repository.Update(account);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Cari silindi : {AccountId} - {AccountName}", account.Id, account.Name);

        return true;
    }
}