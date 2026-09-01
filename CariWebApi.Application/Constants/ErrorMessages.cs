namespace CariWebApi.Application.Constants;

public class ErrorMessages
{
    public const string NameRequired = "Ürün adı boş olamaz.";
    
    public const string CodeRequired = "Ürün kodu boş olamaz.";
    
    public const string NegativePrice = "Birim fiyat negatif olamaz.";
    
    public const string CompanyNameRequired = "Şirket adı boş olamaz.";
    
    public const string NotFound = "Kayıt bulunamadı.";
    
    public const string UsernameTaken = "Bu kullanıcı adı zaten kullanılıyor.";
    
    public const string InvalidCredentials = "Kullanıcı adı veya şifre hatalı.";
    
    public const string NoCompanySelected = "Bu işlem için önce bir şirket oluşturmalısınız veya bir şirket seçmelisiniz.";
    
    public const string NoAccessToCompany = "Bu şirkete erişiminiz yok.";
    
    public const string InvalidReceiptType = "Geçersiz fiş türü. 'Sales' veya 'Purchase' olmalı.";
   
    public const string ReceiptAlreadyApproved = "Onaylanmış bir fişe satır eklenemez/değiştirilemez.";
    
    public const string StockNotFound = "Belirtilen stok bulunamadı.";
    
    public const string InvalidQuantity = "Miktar sıfırdan büyük olmalı.";
    
    public const string EmptyReceipt = "Boş bir fiş onaylanamaz.";
    
}