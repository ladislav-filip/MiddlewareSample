using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MiddlewareSample.Infrastructure;

namespace MiddlewareSample.Middlewares
{
    public class LanguageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LanguageMiddleware> _logger;

        public LanguageMiddleware(RequestDelegate next, ILogger<LanguageMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("start language middleware...");
            var jazyk = context.Request.Query["lng"];
            
            if (!string.IsNullOrEmpty(jazyk))
            {
                var culture = new CultureInfo(jazyk);
                CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = culture;
            }

            await _next(context);
        }
    }
}