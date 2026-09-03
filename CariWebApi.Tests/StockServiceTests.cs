using CariWebApi.Application.Constants;
using CariWebApi.Application.DTOs;
using CariWebApi.Application.Interfaces;
using CariWebApi.Application.Services;
using CariWebApi.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MockQueryable;
using MockQueryable.Moq;
using Xunit;

namespace CariWebApi.Tests;

public class StockServiceTests
{
    // ortak sahte nesneleri kolayca kurmak için yardımcı metod
    private static (Mock<IRepository<Stock>> repo, Mock<IMapper> mapper, Mock<ILogger<StockService>> logger, Mock<ICurrentUserService> currentUser) CreateMocks(int? companyId = 1)
    {
        var repo = new Mock<IRepository<Stock>>();
        var mapper = new Mock<IMapper>();
        var logger = new Mock<ILogger<StockService>>();
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.CompanyId).Returns(companyId);
        return (repo, mapper, logger, currentUser);
    }

    // stok adı boşsa hata fırlatmalı
    [Fact]
    public async Task CreateAsync_EmptyName_ThrowsValidationException()
    {
        var (repo, mapper, logger, currentUser) = CreateMocks();
        var stockService = new StockService(repo.Object, mapper.Object, logger.Object, currentUser.Object);

        var dto = new CreateStockDto { Name = "", Code = "ELM001", UnitPrice = 10 };

        await Assert.ThrowsAsync<ValidationException>(() => stockService.CreateAsync(dto));
    }

    // stok kodu boşsa hata fırlatmalı
    [Fact]
    public async Task CreateAsync_EmptyCode_ThrowsValidationException()
    {
        var (repo, mapper, logger, currentUser) = CreateMocks();
        var stockService = new StockService(repo.Object, mapper.Object, logger.Object, currentUser.Object);

        var dto = new CreateStockDto { Name = "Elma", Code = "", UnitPrice = 10 };

        await Assert.ThrowsAsync<ValidationException>(() => stockService.CreateAsync(dto));
    }

    // fiyat negatifse hata fırlatmalı
    [Fact]
    public async Task CreateAsync_NegativePrice_ThrowsValidationException()
    {
        var (repo, mapper, logger, currentUser) = CreateMocks();
        var stockService = new StockService(repo.Object, mapper.Object, logger.Object, currentUser.Object);

        var dto = new CreateStockDto { Name = "Elma", Code = "ELM001", UnitPrice = -5 };

        await Assert.ThrowsAsync<ValidationException>(() => stockService.CreateAsync(dto));
    }

    // aktif şirket yoksa hata fırlatmalı
    [Fact]
    public async Task CreateAsync_NoCompanySelected_ThrowsValidationException()
    {
        var (repo, mapper, logger, currentUser) = CreateMocks(companyId: null);
        var stockService = new StockService(repo.Object, mapper.Object, logger.Object, currentUser.Object);

        var dto = new CreateStockDto { Name = "Elma", Code = "ELM001", UnitPrice = 10 };

        await Assert.ThrowsAsync<ValidationException>(() => stockService.CreateAsync(dto));
    }

    // geçerli bilgilerle stok oluşturma başarılı olmalı
    [Fact]
    public async Task CreateAsync_ValidData_CreatesStock()
    {
        var (repo, mapper, logger, currentUser) = CreateMocks();

        var dto = new CreateStockDto { Name = "Elma", Code = "ELM001", UnitPrice = 10 };
        var stockEntity = new Stock { Id = 1, Name = "Elma", Code = "ELM001", UnitPrice = 10, CompanyId = 1 };
        var resultDto = new StockDto { Id = 1, Name = "Elma", Code = "ELM001", UnitPrice = 10 };

        mapper.Setup(m => m.Map<Stock>(dto)).Returns(stockEntity);
        mapper.Setup(m => m.Map<StockDto>(stockEntity)).Returns(resultDto);

        var stockService = new StockService(repo.Object, mapper.Object, logger.Object, currentUser.Object);

        var result = await stockService.CreateAsync(dto);

        Assert.Equal("Elma", result.Name);
        repo.Verify(r => r.AddAsync(It.IsAny<Stock>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // var olmayan bir stok Id'siyle güncelleme, null dönmeli
    [Fact]
    public async Task UpdateAsync_StockNotFound_ReturnsNull()
    {
        var (repo, mapper, logger, currentUser) = CreateMocks();

        var emptyList = new List<Stock>().BuildMock();
        repo.Setup(r => r.Query()).Returns(emptyList);

        var stockService = new StockService(repo.Object, mapper.Object, logger.Object, currentUser.Object);
        var dto = new UpdateStockDto { Name = "Elma", Code = "ELM001", UnitPrice = 10 };

        var result = await stockService.UpdateAsync(99, dto);

        Assert.Null(result);
    }

    // başka bir şirkete ait stok silinmeye çalışılırsa, false dönmeli
    [Fact]
    public async Task DeleteAsync_StockBelongsToDifferentCompany_ReturnsFalse()
    {
        var (repo, mapper, logger, currentUser) = CreateMocks(companyId: 1);

        var otherCompanyStock = new Stock { Id = 5, CompanyId = 2, Name = "Armut" };
        var stockList = new List<Stock> { otherCompanyStock }.BuildMock();
        repo.Setup(r => r.Query()).Returns(stockList);

        var stockService = new StockService(repo.Object, mapper.Object, logger.Object, currentUser.Object);

        var result = await stockService.DeleteAsync(5);

        Assert.False(result);
    }
}