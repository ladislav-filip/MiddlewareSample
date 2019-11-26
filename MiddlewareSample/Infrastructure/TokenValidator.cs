using Microsoft.Extensions.Logging;

namespace MiddlewareSample.Infrastructure
{
    public class TokenValidator : ITokenValidator
    {
        private readonly ILogger<TokenValidator> _logger;


        public TokenValidator(ILogger<TokenValidator> logger)
        {
            _logger = logger;
        }
        
        public bool IsValid(string token)
        {
            _logger.LogDebug("Token " + token);
            return true;
        }
    }
}