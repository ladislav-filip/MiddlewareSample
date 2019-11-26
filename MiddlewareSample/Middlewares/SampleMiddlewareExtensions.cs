using Microsoft.AspNetCore.Builder;

namespace MiddlewareSample.Middlewares
{
    public static class SampleMiddlewareExtensions
    {
        public static IApplicationBuilder UseToken(this IApplicationBuilder app)
        {
            app.UseMiddleware<TokenMiddleware>();
            return app;
        }

        public static IApplicationBuilder UseLanguage(this IApplicationBuilder app)
        {
            app.UseMiddleware<LanguageMiddleware>();
            return app;
        }
    }
}