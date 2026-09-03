using CariWebApi.Application.Interfaces;
using CariWebApi.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CariWebApi.Application.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return int.Parse(value!);
        }
    }

    public int? CompanyId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirst("companyId")?.Value;
            return value != null ? int.Parse(value) : null;
        }
    }

    public string? Role
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}