using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MiddlewareSample.Middlewares
{
    public class SecondBranchFirstMiddleware
    {
        private readonly RequestDelegate _next;

        public SecondBranchFirstMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("1. second start");
            await _next(context);
            Console.WriteLine("1. second end");
        }
    }
}