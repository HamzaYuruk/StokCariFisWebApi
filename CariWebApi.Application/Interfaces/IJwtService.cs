using CariWebApi.Domain.Entities;

namespace CariWebApi.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user, int? companyId = null, string? role = null);
}