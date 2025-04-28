using CoworkingApp.Services;

namespace CoworkingApp.Middlewares;

public class TokenRefreshMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, IAuthService authService)
    {
        var jwt = context.Request.Cookies["jwt"];
        
        // if JWT is not expired or missing, continue the pipeline
        if (!string.IsNullOrEmpty(jwt))
        {
            await next(context);
            return;
        }

        // token is expired, try to refresh
        try
        {
            var refreshToken = context.Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken)) // Refresh token still not expired
            {
                var success = await authService.TryRefreshToken(context);

                if (success)
                {
                    // refresh was successful, proceed with the request
                    await next(context);
                    return;
                }
                else
                {
                    // clear cookies if refresh fails
                    context.Response.Cookies.Delete("jwt");
                    context.Response.Cookies.Delete("refreshToken");
                }
            }
        }
        catch
        {
            // ignore invalid JWT, let the auth middleware deal with it
        }

        // if refresh failed, return unauthorized
        // context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        // await context.Response.WriteAsync("Token refresh failed");
        await next(context);
    }
}
