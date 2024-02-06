using DAL.Model;
using Humanizer.Localisation.Formatters;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;
using System.Text;

namespace FlapKap_TechnicalChallenge.Middlwares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;


        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;

        }

        public async Task Invoke(HttpContext context, UserManager<User> userManager)
        {
            var token = WebUtility.UrlDecode(context.Request.Cookies["refreshToken"]);
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            try
            {

              
                #region Request 
                var requestBodyStream = new MemoryStream();
                var originalRequestBody = context.Request.Body;
                await context.Request.Body.CopyToAsync(requestBodyStream);
                requestBodyStream.Seek(0, SeekOrigin.Begin);
                var url = UriHelper.GetDisplayUrl(context.Request);
                var requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();
                Log.Information($"REQUEST METHOD: {context.Request.Method}, REQUEST BODY: {requestBodyText}, REQUEST URL: {url}");
                Log.Information($"UserName of him is : {user?.UserName ?? "Anonymous"} ");
                requestBodyStream.Seek(0, SeekOrigin.Begin);
                context.Request.Body = requestBodyStream;
                #endregion


                #region Response
                var bodyStream = context.Response.Body;
                var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;
                await _next(context);
                context.Request.Body = originalRequestBody;
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                var responseBody = new StreamReader(responseBodyStream).ReadToEnd();
                Log.Information($"RESPONSE LOG: {responseBody}");

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(bodyStream);
                #endregion
            }
            catch (Exception ex)
            {

                Log.Error($"Unhandled exception: {ex.Message}");
                Log.Error($"Endpoint: {context.GetEndpoint()?.DisplayName ?? "Unknown endpoint"}");
                Log.Error($"UserName: {user?.UserName ?? "Anonymous"}");
                Log.Error($"Request Method: {context.Request.Method}");
                Log.Error($"Request Path: {context.Request.Path}");
                Log.Error(ex, "Exception details");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
        

    }
}
