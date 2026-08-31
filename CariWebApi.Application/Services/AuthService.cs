using System.Runtime.InteropServices.JavaScript;
using CariWebApi.Application.DTOs.Auth;
using CariWebApi.Application.Interfaces;
using CariWebApi.Domain.Entities;
using CariWebApi.Application.Constants;
using Microsoft.EntityFrameworkCore;

namespace CariWebApi.Application.Services;

public class AuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly JwtService _jwtService;

    public AuthService(IRepository<User> userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }
    
    // register service
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (existingUser != null)
        {
            throw new ValidationException(ErrorMessages.UsernameTaken);
        }

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = PasswordHasher.Hash(dto.Password)
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username
        };
    }
    
    
    // login service
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null)
        {
            throw new ValidationException(ErrorMessages.InvalidCredentials);
        }

        var isPasswordValid = PasswordHasher.Verify(dto.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new ValidationException(ErrorMessages.InvalidCredentials);
        }

        var token = _jwtService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username
        };
    }
}