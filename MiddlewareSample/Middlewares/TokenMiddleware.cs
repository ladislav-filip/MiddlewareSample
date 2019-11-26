using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MiddlewareSample.Infrastructure;

namespace MiddlewareSample.Middlewares
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenMiddleware> _logger;

        public TokenMiddleware(RequestDelegate next, ILogger<TokenMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// V této metodě je možné využít dependency injection, podobně jako v konstruktoru
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tokenValidator"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, ITokenValidator tokenValidator)
        {
            var token = context.Request.Headers["token"];
            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogDebug($"Token je: {token}");
                if (tokenValidator.IsValid(token))
                {
                    _logger.LogDebug("Token je OK.");
                }
            }
            else
            {
                _logger.LogDebug("Token neni.");
            }
            
            await _next(context);
        }
    }
}