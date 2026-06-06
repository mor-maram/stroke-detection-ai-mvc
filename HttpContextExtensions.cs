public static class HttpContextExtensions
{
    public static string? GetUserIP(this HttpContext context)
    {
        return context.Connection?.RemoteIpAddress?.ToString();
    }
}
