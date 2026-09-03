using CariWebApi.Application.Constants;
using CariWebApi.Application.DTOs;
using CariWebApi.Application.Interfaces;
using CariWebApi.Application.Services;
using CariWebApi.Domain.Entities;
using CariWebApi.Domain.Enums;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MockQueryable;
using MockQueryable.Moq;
using Xunit;

namespace CariWebApi.Tests;

public class ReceiptServiceTests
{
    // ortak sahte nesneleri kolayca kurmak için yardımcı metod
    private static (Mock<IRepository<Receipt>> repo, Mock<IRepository<ReceiptDetail>> detailRepo,
        Mock<IRepository<Stock>> stockRepo, Mock<IRepository<StockTrans>> stockTransRepo,
        Mock<IRepository<Account>> accountRepo, Mock<IRepository<ActTrans>> actTransRepo,
        Mock<IMapper> mapper, Mock<ILogger<ReceiptService>> logger,
        Mock<ICurrentUserService> currentUser) CreateMocks(int? companyId = 1)
    {
        var repo = new Mock<IRepository<Receipt>>();
        var detailRepo = new Mock<IRepository<ReceiptDetail>>();
        var stockRepo = new Mock<IRepository<Stock>>();
        var stockTransRepo = new Mock<IRepository<StockTrans>>();
        var accountRepo = new Mock<IRepository<Account>>();
        var actTransRepo = new Mock<IRepository<ActTrans>>();
        var mapper = new Mock<IMapper>();
        var logger = new Mock<ILogger<ReceiptService>>();
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.CompanyId).Returns(companyId);

        return (repo, detailRepo, stockRepo, stockTransRepo, accountRepo, actTransRepo, mapper, logger, currentUser);
    }

    // geçersiz fiş tipi gönderilirse hata fırlatmalı
    [Fact]
    public async Task CreateAsync_InvalidReceiptType_ThrowsValidationException()
    {
        var (repo, detailRepo, stockRepo, stockTransRepo, accountRepo, actTransRepo, mapper, logger, currentUser) = CreateMocks();

        var receiptService = new ReceiptService(
            repo.Object, detailRepo.Object, stockRepo.Object, stockTransRepo.Object,
            accountRepo.Object, actTransRepo.Object, mapper.Object, logger.Object, currentUser.Object);

        var dto = new CreateReceiptDto { AccountId = 1, ReceiptType = "GecersizTur", Date = DateTime.UtcNow };

        await Assert.ThrowsAsync<ValidationException>(() => receiptService.CreateAsync(dto));
    }

    // zaten onaylanmış bir fişe tekrar onay verilmeye çalışılırsa hata fırlatmalı
    [Fact]
    public async Task ApproveAsync_AlreadyApproved_ThrowsValidationException()
    {
        var (repo, detailRepo, stockRepo, stockTransRepo, accountRepo, actTransRepo, mapper, logger, currentUser) = CreateMocks();

        var approvedReceipt = new Receipt
        {
            Id = 1,
            CompanyId = 1,
            Status = ReceiptStatus.Approved,
            Details = new List<ReceiptDetail>()
        };
        var receiptList = new List<Receipt> { approvedReceipt }.BuildMock();
        repo.Setup(r => r.Query()).Returns(receiptList);

        var receiptService = new ReceiptService(
            repo.Object, detailRepo.Object, stockRepo.Object, stockTransRepo.Object,
            accountRepo.Object, actTransRepo.Object, mapper.Object, logger.Object, currentUser.Object);

        await Assert.ThrowsAsync<ValidationException>(() => receiptService.ApproveAsync(1));
    }

    // hiç satırı olmayan bir fiş onaylanmaya çalışılırsa hata fırlatmalı
    [Fact]
    public async Task ApproveAsync_EmptyReceipt_ThrowsValidationException()
    {
        var (repo, detailRepo, stockRepo, stockTransRepo, accountRepo, actTransRepo, mapper, logger, currentUser) = CreateMocks();

        var draftReceipt = new Receipt
        {
            Id = 1,
            CompanyId = 1,
            Status = ReceiptStatus.Draft,
            Details = new List<ReceiptDetail>()
        };
        var receiptList = new List<Receipt> { draftReceipt }.BuildMock();
        repo.Setup(r => r.Query()).Returns(receiptList);

        var receiptService = new ReceiptService(
            repo.Object, detailRepo.Object, stockRepo.Object, stockTransRepo.Object,
            accountRepo.Object, actTransRepo.Object, mapper.Object, logger.Object, currentUser.Object);

        await Assert.ThrowsAsync<ValidationException>(() => receiptService.ApproveAsync(1));
    }

    // satış fişinde, depoda yeterli stok yoksa hata fırlatmalı
    [Fact]
    public async Task ApproveAsync_InsufficientStock_ThrowsValidationException()
    {
        var (repo, detailRepo, stockRepo, stockTransRepo, accountRepo, actTransRepo, mapper, logger, currentUser) = CreateMocks();

        var stock = new Stock { Id = 5, Name = "Elma", Balance = 10, CompanyId = 1 };
        var detail = new ReceiptDetail { Id = 1, StockId = 5, Quantity = 100, UnitPrice = 10, LineTotal = 1000 };
        var draftReceipt = new Receipt
        {
            Id = 1,
            CompanyId = 1,
            AccountId = 1,
            ReceiptType = ReceiptType.Sales,
            Status = ReceiptStatus.Draft,
            TotalAmount = 1000,
            Details = new List<ReceiptDetail> { detail }
        };

        var receiptList = new List<Receipt> { draftReceipt }.BuildMock();
        repo.Setup(r => r.Query()).Returns(receiptList);
        stockRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(stock);

        var receiptService = new ReceiptService(
            repo.Object, detailRepo.Object, stockRepo.Object, stockTransRepo.Object,
            accountRepo.Object, actTransRepo.Object, mapper.Object, logger.Object, currentUser.Object);

        await Assert.ThrowsAsync<ValidationException>(() => receiptService.ApproveAsync(1));
    }

    // geçerli bir alış fişi onaylanınca, stok bakiyesi artmalı
    [Fact]
    public async Task ApproveAsync_ValidPurchase_IncreasesStockBalance()
    {
        var (repo, detailRepo, stockRepo, stockTransRepo, accountRepo, actTransRepo, mapper, logger, currentUser) = CreateMocks();

        var stock = new Stock { Id = 5, Name = "Elma", Balance = 10, CompanyId = 1 };
        var account = new Account { Id = 1, Name = "Mehmet", Balance = 0, CompanyId = 1 };
        var detail = new ReceiptDetail { Id = 1, StockId = 5, Quantity = 20, UnitPrice = 10, LineTotal = 200 };
        var draftReceipt = new Receipt
        {
            Id = 1,
            CompanyId = 1,
            AccountId = 1,
            ReceiptType = ReceiptType.Purchase,
            Status = ReceiptStatus.Draft,
            TotalAmount = 200,
            Details = new List<ReceiptDetail> { detail }
        };

        var receiptList = new List<Receipt> { draftReceipt }.BuildMock();
        repo.Setup(r => r.Query()).Returns(receiptList);
        stockRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(stock);
        accountRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

        var receiptService = new ReceiptService(
            repo.Object, detailRepo.Object, stockRepo.Object, stockTransRepo.Object,
            accountRepo.Object, actTransRepo.Object, mapper.Object, logger.Object, currentUser.Object);

        await receiptService.ApproveAsync(1);

        // Purchase: stok 10'dan, +20 ile 30'a çıkmalı
        Assert.Equal(30, stock.Balance);
        // Purchase: hesap bakiyesi, -200 olmalı (Credit=200, Debit=0 → Balance = 0 - 200)
        Assert.Equal(-200, account.Balance);
        Assert.Equal(ReceiptStatus.Approved, draftReceipt.Status);
    }
}