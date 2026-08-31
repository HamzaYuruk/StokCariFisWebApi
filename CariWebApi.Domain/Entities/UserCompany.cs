using CariWebApi.Domain.Enums;

namespace CariWebApi.Domain.Entities;

public class UserCompany
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int CompanyId { get; set; }
    public Company? Company { get; set; }

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;
}