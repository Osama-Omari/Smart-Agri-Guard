using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace WebAPILayer.Filters
{
    public class ApiKeyAuthAttribute : Attribute,IAsyncActionFilter
    {
        private const string Header_Name = "X-API-KEY";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            if (!context.HttpContext.Request.Headers.TryGetValue(Header_Name, out var extractedKey))
            {
                context.Result = new UnauthorizedObjectResult("API Key missing");
                return;
            }

            var validKeys = config.GetSection("ApiKeys").GetChildren().Select(x => x.Value).ToList();
            if (!validKeys.Contains(extractedKey))
            {
                context.Result = new UnauthorizedObjectResult("Unauthorized client");
                return;
            }

            await next();
        }
    }
}
