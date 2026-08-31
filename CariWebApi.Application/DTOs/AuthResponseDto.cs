namespace CariWebApi.Application.DTOs.Auth;

// Login/Register başarılı olunca kullanıcıya geri döneceğimiz responsu tanımlıyor.

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
}