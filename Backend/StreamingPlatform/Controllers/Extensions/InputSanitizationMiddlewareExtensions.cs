using StreamingPlatform.Controllers.Middleware;

namespace StreamingPlatform.Controllers.Extensions
{
    public static class InputSanitizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseInputSanitization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InputSanitizationMiddleware>();
        }
    }

}
