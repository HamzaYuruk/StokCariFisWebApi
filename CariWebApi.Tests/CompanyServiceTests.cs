using CariWebApi.Application.DTOs;
using CariWebApi.Application.Constants;
using CariWebApi.Application.Interfaces;
using CariWebApi.Application.Services;
using CariWebApi.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MockQueryable;


namespace CariWebApi.Tests;

public class CompanyServiceTests
{
    // ortak sahte nesneleri kolayca kurmak için yardımcı metod
    private static (Mock<IRepository<Company>> repo, Mock<IRepository<UserCompanyRole>> userCompanyRepo,
        Mock<IRepository<User>> userRepo, Mock<IRepository<Role>> roleRepo,
        Mock<IMapper> mapper, Mock<ILogger<CompanyService>> logger,
        Mock<IJwtService> jwtService) CreateMocks()
    {
        var repo = new Mock<IRepository<Company>>();
        var userCompanyRepo = new Mock<IRepository<UserCompanyRole>>();
        var userRepo = new Mock<IRepository<User>>();
        var roleRepo = new Mock<IRepository<Role>>();
        var mapper = new Mock<IMapper>();
        var logger = new Mock<ILogger<CompanyService>>();
        var jwtService = new Mock<IJwtService>();

        return (repo, userCompanyRepo, userRepo, roleRepo, mapper, logger, jwtService);
    }

    // şirket adı boşsa hata fırlatmalı
    [Fact]
    public async Task CreateAsync_EmptyName_ThrowsValidationException()
    {
        var (repo, userCompanyRepo, userRepo, roleRepo, mapper, logger, jwtService) = CreateMocks();

        var companyService = new CompanyService(
            repo.Object, userCompanyRepo.Object, userRepo.Object, roleRepo.Object,
            mapper.Object, logger.Object, jwtService.Object);

        var dto = new CreateCompanyDto { Name = "" };

        await Assert.ThrowsAsync<ValidationException>(() => companyService.CreateAsync(dto, 1));
    }

    // geçerli veriyle şirket oluşturulunca, Owner rolü atanmalı ve token dönmeli
    [Fact]
    public async Task CreateAsync_ValidData_CreatesCompanyWithOwnerRole()
    {
        var (repo, userCompanyRepo, userRepo, roleRepo, mapper, logger, jwtService) = CreateMocks();

        var dto = new CreateCompanyDto { Name = "Ahmet Ticaret" };
        var companyEntity = new Company { Id = 1, Name = "Ahmet Ticaret" };
        var resultDto = new CompanyDto { Id = 1, Name = "Ahmet Ticaret" };
        var ownerRole = new Role { Id = 1, Name = "Owner" };
        var user = new User { Id = 5, Username = "ahmet" };

        mapper.Setup(m => m.Map<Company>(dto)).Returns(companyEntity);
        mapper.Setup(m => m.Map<CompanyDto>(companyEntity)).Returns(resultDto);

        var roleList = new List<Role> { ownerRole }.BuildMock();
        roleRepo.Setup(r => r.Query()).Returns(roleList);

        userRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(user);

        jwtService.Setup(j => j.GenerateToken(user, 1, "Owner")).Returns("sahte-token");

        var companyService = new CompanyService(
            repo.Object, userCompanyRepo.Object, userRepo.Object, roleRepo.Object,
            mapper.Object, logger.Object, jwtService.Object);

        var result = await companyService.CreateAsync(dto, 5);

        Assert.Equal("sahte-token", result.Token);
        Assert.Equal("Ahmet Ticaret", result.Company.Name);
        userCompanyRepo.Verify(r => r.AddAsync(It.Is<UserCompanyRole>(
            uc => uc.UserId == 5 && uc.CompanyId == 1 && uc.RoleId == 1)), Times.Once);
    }

    // kullanıcının erişimi olmayan bir şirketi seçmeye çalışması, hata fırlatmalı
    [Fact]
    public async Task SelectCompanyAsync_NoAccess_ThrowsValidationException()
    {
        var (repo, userCompanyRepo, userRepo, roleRepo, mapper, logger, jwtService) = CreateMocks();

        var emptyList = new List<UserCompanyRole>().BuildMock();
        userCompanyRepo.Setup(r => r.Query()).Returns(emptyList);

        var companyService = new CompanyService(
            repo.Object, userCompanyRepo.Object, userRepo.Object, roleRepo.Object,
            mapper.Object, logger.Object, jwtService.Object);

        await Assert.ThrowsAsync<ValidationException>(() => companyService.SelectCompanyAsync(99, 5));
    }

    // kullanıcının erişimi olan bir şirketi seçmesi başarılı olmalı, doğru rolle token dönmeli
    [Fact]
    public async Task SelectCompanyAsync_HasAccess_ReturnsTokenWithCorrectRole()
    {
        var (repo, userCompanyRepo, userRepo, roleRepo, mapper, logger, jwtService) = CreateMocks();

        var customerRole = new Role { Id = 3, Name = "Customer" };
        var userCompanyRole = new UserCompanyRole
        {
            UserId = 5,
            CompanyId = 2,
            RoleId = 3,
            Role = customerRole,
            IsActive = true
        };
        var linkList = new List<UserCompanyRole> { userCompanyRole }.BuildMock();
        userCompanyRepo.Setup(r => r.Query()).Returns(linkList);

        var company = new Company { Id = 2, Name = "Fatma A.Ş." };
        var resultDto = new CompanyDto { Id = 2, Name = "Fatma A.Ş." };
        var user = new User { Id = 5, Username = "mehmet" };

        repo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(company);
        userRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(user);
        mapper.Setup(m => m.Map<CompanyDto>(company)).Returns(resultDto);
        jwtService.Setup(j => j.GenerateToken(user, 2, "Customer")).Returns("sahte-token-2");

        var companyService = new CompanyService(
            repo.Object, userCompanyRepo.Object, userRepo.Object, roleRepo.Object,
            mapper.Object, logger.Object, jwtService.Object);

        var result = await companyService.SelectCompanyAsync(2, 5);

        Assert.Equal("sahte-token-2", result.Token);
        Assert.Equal("Fatma A.Ş.", result.Company.Name);
    }
}