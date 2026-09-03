

namespace CariWebApi.Domain.Entities;

public class UserCompanyRole
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int CompanyId { get; set; }
    public Company? Company { get; set; }

    public int RoleId { get; set; }
    public Role? Role { get; set; }

    public bool IsActive { get; set; } = true;
}