using FoodioAPI.Database.Repositories;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;

namespace FoodioAPI.Middlewares
{
    public class TableTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TableTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
        {
            if ((context.Request.Path.StartsWithSegments("/api/dinein") ||
                context.Request.Path.StartsWithSegments("/api/qr")) &&
                context.Request.Headers.TryGetValue("access-token-key", out var tokenValue))
            {
                var session = await unitOfWork.OrderSessionRepo
                    .GetActiveSessionByTokenAsync(tokenValue);

                if (session == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid or expired table token.");
                    return;
                }

                context.Items["TableId"] = session.TableId;
                context.Items["SessionId"] = session.Id;
                Console.WriteLine($"Received token: {tokenValue}");
                Console.WriteLine($"Found session: {session?.Id}, TableId: {session?.TableId}");

            }

            await _next(context);
        }
    }
}
