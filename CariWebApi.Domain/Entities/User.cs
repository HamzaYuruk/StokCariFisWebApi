using CariWebApi.Domain.Enums;

namespace CariWebApi.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
}