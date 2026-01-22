var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Handle all HTTP methods with a catch-all route
var allMethods = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS", "TRACE", "CONNECT" };
app.MapMethods("{**path}", allMethods, async (HttpContext context) =>
{
    string body = "";
    try
    {
        using var reader = new StreamReader(context.Request.Body);
        body = await reader.ReadToEndAsync();
    }
    catch
    {
        // leave empty if body cannot be read
    }

    var data = new
    {
        method = context.Request.Method,
        path = context.Request.Path.Value ?? "",
        pathBase = context.Request.PathBase.Value ?? "",
        queryString = context.Request.QueryString.Value ?? "",
        query = context.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToArray()),
        headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToArray()),
        body,
        scheme = context.Request.Scheme,
        host = context.Request.Host.Value,
        protocol = context.Request.Protocol,
        contentType = context.Request.ContentType,
        contentLength = context.Request.ContentLength,
        remoteIp = context.Connection.RemoteIpAddress?.ToString(),
    };

    context.Response.ContentType = "application/json; charset=utf-8";
    await context.Response.WriteAsJsonAsync(data);
});

app.Run();
