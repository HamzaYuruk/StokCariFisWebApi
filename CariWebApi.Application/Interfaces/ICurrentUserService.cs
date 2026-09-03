using CariWebApi.Domain.Enums;

namespace CariWebApi.Application.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; }
    
    int? CompanyId { get; }
    
    string? Role { get; }
}