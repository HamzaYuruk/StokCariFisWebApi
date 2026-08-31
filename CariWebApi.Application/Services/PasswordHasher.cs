namespace CariWebApi.Application.Services;

public static class PasswordHasher
{
    // gelen şifreyi hashleyip geri döndürür
    public static string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // gelen şifre hash'e eşit mi diye kontrol etme
    public static bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}