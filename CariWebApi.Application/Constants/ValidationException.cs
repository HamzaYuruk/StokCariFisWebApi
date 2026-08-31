namespace CariWebApi.Application.Constants;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}