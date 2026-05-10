var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://localhost:7235/"); // API port
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // Cookies must stay in each browser, not in the shared server-side API client.
    UseCookies = false
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapPost("/api/auth/client", (HttpContext context, IHttpClientFactory factory) =>
    ProxyAsync(context, factory, HttpMethod.Post, "api/auth/client"));

app.MapGet("/api/profile", (HttpContext context, IHttpClientFactory factory) =>
    ProxyAsync(context, factory, HttpMethod.Get, "api/profile"));

app.MapPost("/api/profile/orders/{orderId:int}/cancel", (int orderId, HttpContext context, IHttpClientFactory factory) =>
    ProxyAsync(context, factory, HttpMethod.Post, $"api/profile/orders/{orderId}/cancel"));

app.MapGet("/api/orders/pickup-slots", (HttpContext context, IHttpClientFactory factory) =>
    ProxyAsync(context, factory, HttpMethod.Get, "api/orders/pickup-slots"));

app.MapPost("/api/orders", (HttpContext context, IHttpClientFactory factory) =>
    ProxyAsync(context, factory, HttpMethod.Post, "api/orders"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authorization}/{action=AuthorizationView}/{id?}");

app.Run();

static async Task<IResult> ProxyAsync(
    HttpContext context,
    IHttpClientFactory factory,
    HttpMethod method,
    string apiPath)
{
    using var request = new HttpRequestMessage(method, apiPath);

    if (context.Request.Headers.TryGetValue("Cookie", out var cookieHeader))
    {
        request.Headers.TryAddWithoutValidation("Cookie", cookieHeader.ToArray());
    }

    if (HttpMethods.IsPost(context.Request.Method)
        || HttpMethods.IsPut(context.Request.Method)
        || HttpMethods.IsPatch(context.Request.Method))
    {
        request.Content = new StreamContent(context.Request.Body);
        if (!string.IsNullOrWhiteSpace(context.Request.ContentType))
        {
            request.Content.Headers.TryAddWithoutValidation("Content-Type", context.Request.ContentType);
        }
    }

    var client = factory.CreateClient("api");
    using var response = await client.SendAsync(
        request,
        HttpCompletionOption.ResponseHeadersRead,
        context.RequestAborted);

    if (response.Headers.TryGetValues("Set-Cookie", out var setCookies))
    {
        context.Response.Headers.Append("Set-Cookie", setCookies.ToArray());
    }

    var content = response.Content == null
        ? string.Empty
        : await response.Content.ReadAsStringAsync(context.RequestAborted);
    var contentType = response.Content?.Headers.ContentType?.ToString();

    return Results.Content(content, contentType, statusCode: (int)response.StatusCode);
}
