namespace X.WebApi.Middlewares;
public class ErrorHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var errorResponse = new { Message = "An unexpected error occurred.", Details = ex.Message };
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}