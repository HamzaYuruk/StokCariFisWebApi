using AutoMapper;
using CariWebApi.Application.Constants;
using CariWebApi.Application.DTOs;
using CariWebApi.Application.Interfaces;
using CariWebApi.Domain.Entities;
using CariWebApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CariWebApi.Application.Services;

public class ReceiptService : IReceiptService
{
    private readonly IRepository<Receipt> _repository;
    private readonly IRepository<ReceiptDetail> _detailRepository;
    private readonly IRepository<Stock> _stockRepository;
    private readonly IRepository<StockTrans> _stockTransRepository;
    private readonly IRepository<Account> _accountRepository;
    private readonly IRepository<ActTrans> _actTransRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ReceiptService> _logger;
    private readonly ICurrentUserService _currentUser;

    public ReceiptService(
        IRepository<Receipt> repository,
        IRepository<ReceiptDetail> detailRepository,
        IRepository<Stock> stockRepository,
        IRepository<StockTrans> stockTransRepository,
        IRepository<Account> accountRepository,
        IRepository<ActTrans> actTransRepository,
        IMapper mapper,
        ILogger<ReceiptService> logger,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _detailRepository = detailRepository;
        _stockRepository = stockRepository;
        _stockTransRepository = stockTransRepository;
        _accountRepository = accountRepository;
        _actTransRepository = actTransRepository;
        _mapper = mapper;
        _logger = logger;
        _currentUser = currentUser;
    }
    
    // fiş kesme
    public async Task<ReceiptDto> CreateAsync(CreateReceiptDto dto)
    {
        if (_currentUser.CompanyId == null)
        {
            throw new ValidationException(ErrorMessages.NoCompanySelected);
        }

        if (!Enum.TryParse<ReceiptType>(dto.ReceiptType, true, out var receiptType))
        {
            throw new ValidationException(ErrorMessages.InvalidReceiptType);
        }

        var count = await _repository.Query()
            .CountAsync(r => r.CompanyId == _currentUser.CompanyId && r.ReceiptType == receiptType);

        var receiptNumber = $"{receiptType}-{(count + 1):D5}";

        var receipt = new Receipt
        {
            CompanyId = _currentUser.CompanyId.Value,
            AccountId = dto.AccountId,
            ReceiptType = receiptType,
            Date = dto.Date,
            ReceiptNumber = receiptNumber,
            Status = ReceiptStatus.Draft,
            TotalAmount = 0
        };

        await _repository.AddAsync(receipt);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Yeni fiş oluşturuldu: {ReceiptId} - {ReceiptNumber}", receipt.Id, receiptNumber);

        return _mapper.Map<ReceiptDto>(receipt);
    }
    
    
    // fişe detay kesme
    public async Task<ReceiptDto?> AddDetailAsync(int receiptId, AddReceiptDetailDto dto)
    {
        var receipt = await _repository.Query()
            .Include(r => r.Details)
            .FirstOrDefaultAsync(r => r.Id == receiptId && r.CompanyId == _currentUser.CompanyId);

        if (receipt == null)
        {
            _logger.LogWarning("Detay eklenmek istenen fiş bulunamadı: {ReceiptId}", receiptId);
            return null;
        }

        if (receipt.Status == ReceiptStatus.Approved)
        {
            throw new ValidationException(ErrorMessages.ReceiptAlreadyApproved);
        }

        var stock = await _stockRepository.Query()
            .FirstOrDefaultAsync(s => s.Id == dto.StockId && s.CompanyId == _currentUser.CompanyId);

        if (stock == null)
        {
            throw new ValidationException(ErrorMessages.StockNotFound);
        }

        if (dto.Quantity <= 0)
        {
            throw new ValidationException(ErrorMessages.InvalidQuantity);
        }

        var detail = new ReceiptDetail
        {
            ReceiptId = receiptId,
            StockId = dto.StockId,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            LineTotal = dto.Quantity * dto.UnitPrice
        };

        await _detailRepository.AddAsync(detail);

        receipt.TotalAmount += detail.LineTotal;
        _repository.Update(receipt);

        await _detailRepository.SaveChangesAsync();

        _logger.LogInformation("Fişe satır eklendi: ReceiptId {ReceiptId}, StockId {StockId}, Quantity {Quantity}", receiptId, dto.StockId, dto.Quantity);

        return _mapper.Map<ReceiptDto>(receipt);
    }
    
    // fişleri getirme
    public async Task<List<ReceiptDto>> GetAllAsync(int page, int pageSize)
    {
        var receipts = await _repository.Query()
            .Include(r => r.Account)
            .Include(r => r.Details)
            .Where(r => r.CompanyId == _currentUser.CompanyId && !r.IsDeleted)
            .OrderByDescending(r => r.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return _mapper.Map<List<ReceiptDto>>(receipts);
    }
    
    // id'si verilen fişi getirme
    public async Task<ReceiptDto?> GetByIdAsync(int id)
    {
        var receipt = await _repository.Query()
            .Include(r => r.Account)
            .Include(r => r.Details)
            .FirstOrDefaultAsync(r => r.Id == id && r.CompanyId == _currentUser.CompanyId && !r.IsDeleted);

        if (receipt == null)
        {
            return null;
        }
        return _mapper.Map<ReceiptDto>(receipt);
    }
    
    // onaylanmamış fişi update
    public async Task<ReceiptDto?> UpdateAsync(int id, CreateReceiptDto dto)
    {
        var receipt = await _repository.Query()
            .FirstOrDefaultAsync(r => r.Id == id && r.CompanyId == _currentUser.CompanyId);

        if (receipt == null)
        {
            _logger.LogWarning("Güncellenmek istenen fiş bulunamadı: {ReceiptId}", id);
            return null;
        }

        if (receipt.Status == ReceiptStatus.Approved)
        {
            throw new ValidationException(ErrorMessages.ReceiptAlreadyApproved);
        }

        if (!Enum.TryParse<ReceiptType>(dto.ReceiptType, true, out var receiptType))
        {
            throw new ValidationException(ErrorMessages.InvalidReceiptType);
        }

        receipt.AccountId = dto.AccountId;
        receipt.ReceiptType = receiptType;
        receipt.Date = dto.Date;

        _repository.Update(receipt);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Fiş güncellendi: {ReceiptId}", id);

        return _mapper.Map<ReceiptDto>(receipt);
    }
    
    // onaylanmamış fişi delete
    public async Task<bool> DeleteAsync(int id)
    {
        var receipt = await _repository.Query()
            .Include(r => r.Details)
            .FirstOrDefaultAsync(r => r.Id == id && r.CompanyId == _currentUser.CompanyId);

        if (receipt == null)
        {
            _logger.LogWarning("Silinmek istenen fiş bulunamadı: {ReceiptId}", id);
            return false;
        }

        if (receipt.Status == ReceiptStatus.Approved)
        {
            throw new ValidationException(ErrorMessages.ReceiptAlreadyApproved);
        }

        foreach (var detail in receipt.Details)
        {
            _detailRepository.Delete(detail);
        }
        _repository.Delete(receipt);

        await _repository.SaveChangesAsync();

        _logger.LogInformation("Taslak fiş kalıcı olarak silindi: {ReceiptId}", id);

        return true;
    }
    
    // fiş onaylama
    public async Task<ReceiptDto?> ApproveAsync(int id)
    {
    var receipt = await _repository.Query()
        .Include(r => r.Details)
        .FirstOrDefaultAsync(r => r.Id == id && r.CompanyId == _currentUser.CompanyId);

    if (receipt == null)
    {
        return null;
    }

    if (receipt.Status == ReceiptStatus.Approved)
    {
        throw new ValidationException(ErrorMessages.ReceiptAlreadyApproved);
    }

    if (receipt.Details.Count == 0)
    {
        throw new ValidationException(ErrorMessages.EmptyReceipt);
    }

    await CreateStockTransactionsAsync(receipt);
    await CreateAccountTransactionAsync(receipt);

    receipt.Status = ReceiptStatus.Approved;
    _repository.Update(receipt);
    await _repository.SaveChangesAsync();

    _logger.LogInformation("Fiş onaylandı: {ReceiptId}", id);

    return _mapper.Map<ReceiptDto>(receipt);
    }
    
    // stocktransaction oluşturma ve onaylanan fişten sonra stoğu güncelleme
   private async Task CreateStockTransactionsAsync(Receipt receipt)
{
    foreach (var detail in receipt.Details)
    {
        var stock = await _stockRepository.GetByIdAsync(detail.StockId);

        var quantity = receipt.ReceiptType == ReceiptType.Purchase
            ? detail.Quantity
            : -detail.Quantity;

        if (receipt.ReceiptType == ReceiptType.Sales && stock!.Balance < detail.Quantity)
        {
            throw new ValidationException($"{stock.Name} için yeterli stok yok.");
        }

        var stockTrans = new StockTrans
        {
            CompanyId = receipt.CompanyId,
            StockId = detail.StockId,
            ReceiptId = receipt.Id,
            Quantity = quantity,
            TransDate = DateTime.UtcNow
        };
        await _stockTransRepository.AddAsync(stockTrans);

        stock!.Balance += quantity;
        _stockRepository.Update(stock);
    }
}

// AccountTransaction oluşturma ve onaylanan fişten sonra account'un balencını güncelleme
   private async Task CreateAccountTransactionAsync(Receipt receipt)
{
    var account = await _accountRepository.GetByIdAsync(receipt.AccountId);

    var actTrans = new ActTrans
    {
        CompanyId = receipt.CompanyId,
        AccountId = receipt.AccountId,
        ReceiptId = receipt.Id,
        Debit = receipt.ReceiptType == ReceiptType.Sales ? receipt.TotalAmount : 0,
        Credit = receipt.ReceiptType == ReceiptType.Purchase ? receipt.TotalAmount : 0,
        TransDate = DateTime.UtcNow
    };
    await _actTransRepository.AddAsync(actTrans);

    account!.Balance += actTrans.Debit - actTrans.Credit;
    _accountRepository.Update(account);
}
}