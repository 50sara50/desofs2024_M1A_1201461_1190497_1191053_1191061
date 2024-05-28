using System.Text;
using Ganss.Xss;

namespace StreamingPlatform.Controllers.Middleware
{
    public class InputSanitizationMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate next = next;
        private readonly HtmlSanitizer sanitizer = new();

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();

            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
                using StreamReader reader = new(context.Request.Body, Encoding.UTF8, true, 1024, true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Seek(0, SeekOrigin.Begin);
                var sanitizedBody = this.sanitizer.Sanitize(body);
                var bytes = Encoding.UTF8.GetBytes(sanitizedBody);
                context.Request.Body = new MemoryStream(bytes);
            }

            await this.next(context);
        }
    }
}
