using CariWebApi.Application.Constants;
using CariWebApi.Application.DTOs;
using CariWebApi.Application.Constants;
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

public class AccountServiceTests
{
    // ortak sahte nesneleri kolayca kurmak için yardımcı metod
    private static (Mock<IRepository<Account>> repo, Mock<IRepository<User>> userRepo,
        Mock<IRepository<UserCompanyRole>> userCompanyRepo, Mock<IRepository<Role>> roleRepo,
        Mock<IRepository<Receipt>> receiptRepo, Mock<IMapper> mapper,
        Mock<ILogger<AccountService>> logger, Mock<ICurrentUserService> currentUser) CreateMocks(int? companyId = 1)
    {
        var repo = new Mock<IRepository<Account>>();
        var userRepo = new Mock<IRepository<User>>();
        var userCompanyRepo = new Mock<IRepository<UserCompanyRole>>();
        var roleRepo = new Mock<IRepository<Role>>();
        var receiptRepo = new Mock<IRepository<Receipt>>();
        var mapper = new Mock<IMapper>();
        var logger = new Mock<ILogger<AccountService>>();
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(c => c.CompanyId).Returns(companyId);

        return (repo, userRepo, userCompanyRepo, roleRepo, receiptRepo, mapper, logger, currentUser);
    }

    // hesap adı boşsa hata fırlatmalı
    [Fact]
    public async Task CreateAsync_EmptyName_ThrowsValidationException()
    {
        var (repo, userRepo, userCompanyRepo, roleRepo, receiptRepo, mapper, logger, currentUser) = CreateMocks();

        var accountService = new AccountService(
            repo.Object, userRepo.Object, userCompanyRepo.Object, roleRepo.Object,
            receiptRepo.Object, mapper.Object, logger.Object, currentUser.Object);

        var dto = new CreateAccountDto { Name = "", Code = "MUS001" };

        await Assert.ThrowsAsync<ValidationException>(() => accountService.CreateAsync(dto));
    }

    // hesap kodu boşsa hata fırlatmalı
    [Fact]
    public async Task CreateAsync_EmptyCode_ThrowsValidationException()
    {
        var (repo, userRepo, userCompanyRepo, roleRepo, receiptRepo, mapper, logger, currentUser) = CreateMocks();

        var accountService = new AccountService(
            repo.Object, userRepo.Object, userCompanyRepo.Object, roleRepo.Object,
            receiptRepo.Object, mapper.Object, logger.Object, currentUser.Object);

        var dto = new CreateAccountDto { Name = "Mehmet", Code = "" };

        await Assert.ThrowsAsync<ValidationException>(() => accountService.CreateAsync(dto));
    }

    // aktif şirket yoksa hata fırlatmalı
    [Fact]
    public async Task CreateAsync_NoCompanySelected_ThrowsValidationException()
    {
        var (repo, userRepo, userCompanyRepo, roleRepo, receiptRepo, mapper, logger, currentUser) = CreateMocks(companyId: null);

        var accountService = new AccountService(
            repo.Object, userRepo.Object, userCompanyRepo.Object, roleRepo.Object,
            receiptRepo.Object, mapper.Object, logger.Object, currentUser.Object);

        var dto = new CreateAccountDto { Name = "Mehmet", Code = "MUS001" };

        await Assert.ThrowsAsync<ValidationException>(() => accountService.CreateAsync(dto));
    }

    // var olmayan kullanıcı adıyla bağlama denemesi, hata fırlatmalı
    [Fact]
    public async Task LinkUserAsync_UserNotFound_ThrowsValidationException()
    {
        var (repo, userRepo, userCompanyRepo, roleRepo, receiptRepo, mapper, logger, currentUser) = CreateMocks();

        var existingAccount = new Account { Id = 1, CompanyId = 1, Name = "Mehmet" };
        var accountList = new List<Account> { existingAccount }.BuildMock();
        repo.Setup(r => r.Query()).Returns(accountList);

        var emptyUserList = new List<User>().BuildMock();
        userRepo.Setup(r => r.Query()).Returns(emptyUserList);

        var accountService = new AccountService(
            repo.Object, userRepo.Object, userCompanyRepo.Object, roleRepo.Object,
            receiptRepo.Object, mapper.Object, logger.Object, currentUser.Object);

        var dto = new LinkAccountUserDto { Username = "olmayankullanici" };

        await Assert.ThrowsAsync<ValidationException>(() => accountService.LinkUserAsync(1, dto));
    }

    // başka şirkete ait hesap için, LinkUserAsync false dönmeli
    [Fact]
    public async Task LinkUserAsync_AccountBelongsToDifferentCompany_ReturnsFalse()
    {
        var (repo, userRepo, userCompanyRepo, roleRepo, receiptRepo, mapper, logger, currentUser) = CreateMocks(companyId: 1);

        var otherCompanyAccount = new Account { Id = 7, CompanyId = 2, Name = "Fatma" };
        var accountList = new List<Account> { otherCompanyAccount }.BuildMock();
        repo.Setup(r => r.Query()).Returns(accountList);

        var accountService = new AccountService(
            repo.Object, userRepo.Object, userCompanyRepo.Object, roleRepo.Object,
            receiptRepo.Object, mapper.Object, logger.Object, currentUser.Object);

        var dto = new LinkAccountUserDto { Username = "mehmet123" };

        var result = await accountService.LinkUserAsync(7, dto);

        Assert.False(result);
    }

    // kullanıcının kendi hesabı yoksa, GetMyAccountAsync null dönmeli
    [Fact]
    public async Task GetMyAccountAsync_NoLinkedAccount_ReturnsNull()
    {
        var (repo, userRepo, userCompanyRepo, roleRepo, receiptRepo, mapper, logger, currentUser) = CreateMocks(companyId: 1);
        currentUser.Setup(c => c.UserId).Returns(99);

        var emptyList = new List<Account>().BuildMock();
        repo.Setup(r => r.Query()).Returns(emptyList);

        var accountService = new AccountService(
            repo.Object, userRepo.Object, userCompanyRepo.Object, roleRepo.Object,
            receiptRepo.Object, mapper.Object, logger.Object, currentUser.Object);

        var result = await accountService.GetMyAccountAsync();

        Assert.Null(result);
    }
}