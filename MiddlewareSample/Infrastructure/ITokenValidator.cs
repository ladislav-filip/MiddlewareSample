namespace MiddlewareSample.Infrastructure
{
    public interface ITokenValidator
    {
        bool IsValid(string token);
    }
}