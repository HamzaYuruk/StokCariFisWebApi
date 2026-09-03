using CariWebApi.Application.DTOs.Auth;
using CariWebApi.Application.Constants;
using CariWebApi.Application.Interfaces;
using CariWebApi.Application.Services;
using CariWebApi.Domain.Entities;
using Moq;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Xunit;

namespace CariWebApi.Tests;

public class AuthServiceTests
{
    // aynı kullanıcı adıyla kayıt olunca hata fırlatmalı
    [Fact]
    public async Task RegisterAsync_UsernameAlreadyExists_ThrowsValidationException()
    {
        var existingUser = new User { Id = 1, Username = "ahmet" };
        var userList = new List<User> { existingUser }.BuildMock();

        var mockUserRepo = new Mock<IRepository<User>>();
        mockUserRepo.Setup(r => r.Query()).Returns(userList);

        var mockJwtService = new Mock<IJwtService>();

        var authService = new AuthService(mockUserRepo.Object, mockJwtService.Object);
        var dto = new RegisterDto { Username = "ahmet", Password = "1234" };

        await Assert.ThrowsAsync<ValidationException>(() => authService.RegisterAsync(dto));
    }

    // yeni bir kullanıcı adıyla kayıt başarılı olmalı, token dönmeli
    [Fact]
    public async Task RegisterAsync_NewUsername_ReturnsTokenAndUserInfo()
    {
        var userList = new List<User> { }.BuildMock();

        var mockUserRepo = new Mock<IRepository<User>>();
        mockUserRepo.Setup(r => r.Query()).Returns(userList);

        var mockJwtService = new Mock<IJwtService>();
        mockJwtService.Setup(j => j.GenerateToken(It.IsAny<User>(), null, null))
            .Returns("sahte-token");

        var authService = new AuthService(mockUserRepo.Object, mockJwtService.Object);
        var dto = new RegisterDto { Username = "mehmet", Password = "1234" };

        var result = await authService.RegisterAsync(dto);

        Assert.Equal("sahte-token", result.Token);
        Assert.Equal("mehmet", result.Username);
    }

    // var olmayan kullanıcı adıyla giriş, hata fırlatmalı
    [Fact]
    public async Task LoginAsync_UserNotFound_ThrowsValidationException()
    {
        var userList = new List<User> { }.BuildMock();

        var mockUserRepo = new Mock<IRepository<User>>();
        mockUserRepo.Setup(r => r.Query()).Returns(userList);

        var mockJwtService = new Mock<IJwtService>();

        var authService = new AuthService(mockUserRepo.Object, mockJwtService.Object);
        var dto = new LoginDto { Username = "olmayankullanici", Password = "1234" };

        await Assert.ThrowsAsync<ValidationException>(() => authService.LoginAsync(dto));
    }

    // yanlış şifreyle giriş, hata fırlatmalı
    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsValidationException()
    {
        var existingUser = new User
        {
            Id = 1,
            Username = "ahmet",
            PasswordHash = PasswordHasher.Hash("dogrusifre")
        };
        var userList = new List<User> { existingUser }.BuildMock();

        var mockUserRepo = new Mock<IRepository<User>>();
        mockUserRepo.Setup(r => r.Query()).Returns(userList);

        var mockJwtService = new Mock<IJwtService>();

        var authService = new AuthService(mockUserRepo.Object, mockJwtService.Object);
        var dto = new LoginDto { Username = "ahmet", Password = "yanlissifre" };

        await Assert.ThrowsAsync<ValidationException>(() => authService.LoginAsync(dto));
    }

    // doğru kullanıcı adı ve şifreyle giriş başarılı olmalı, token dönmeli
    [Fact]
    public async Task LoginAsync_CorrectCredentials_ReturnsToken()
    {
        var existingUser = new User
        {
            Id = 1,
            Username = "ahmet",
            PasswordHash = PasswordHasher.Hash("dogrusifre")
        };
        var userList = new List<User> { existingUser }.BuildMock();

        var mockUserRepo = new Mock<IRepository<User>>();
        mockUserRepo.Setup(r => r.Query()).Returns(userList);

        var mockJwtService = new Mock<IJwtService>();
        mockJwtService.Setup(j => j.GenerateToken(It.IsAny<User>(), null, null))
            .Returns("sahte-token");

        var authService = new AuthService(mockUserRepo.Object, mockJwtService.Object);
        var dto = new LoginDto { Username = "ahmet", Password = "dogrusifre" };

        var result = await authService.LoginAsync(dto);

        Assert.Equal("sahte-token", result.Token);
        Assert.Equal(1, result.UserId);
    }
}