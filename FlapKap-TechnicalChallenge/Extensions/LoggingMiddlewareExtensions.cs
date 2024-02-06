using FlapKap_TechnicalChallenge.Middlwares;

namespace El_BurhanAcademy.Server.Middlewares.Extensions
{
    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
