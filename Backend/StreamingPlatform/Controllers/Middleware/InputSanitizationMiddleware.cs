using System.Text;
using Ganss.Xss;
using Microsoft.Extensions.Primitives;

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
                if (!context.Request.HasFormContentType)
                {
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    using StreamReader reader = new(context.Request.Body, Encoding.UTF8, true, 1024, true);
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    var sanitizedBody = this.sanitizer.Sanitize(body);
                    var bytes = Encoding.UTF8.GetBytes(sanitizedBody);
                    context.Request.Body = new MemoryStream(bytes);
                }
                else
                {
                    IFormCollection form = await context.Request.ReadFormAsync();
                    Dictionary<string, StringValues> sanitized = [];
                    foreach (var key in form.Keys)
                    {
                        var sanitizedKey = this.sanitizer.Sanitize(key);
                        var value = form[key];
                        var sanitizedValue = this.sanitizer.Sanitize(value);
                        sanitized.Add(sanitizedKey, sanitizedValue);
                    }

                    FormCollection sanitizedForm = new(sanitized, form.Files);
                    context.Request.Form = sanitizedForm;
                }
            }

            await this.next(context);
        }
    }
}
