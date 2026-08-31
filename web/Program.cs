// Pro Diesel Worx is a static marketing site. This host exists so the site can be
// published to IIS with Visual Studio Web Deploy; it serves files and nothing else.
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

// The pages carry no fingerprint in their URLs, so make HTML/CSS revalidate every
// time (ETags turn that into a cheap 304) and let images cache properly. Without
// this, browsers apply heuristic caching and can serve a stale page after a deploy.
app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var name = ctx.File.Name;
        var revalidate =
            name.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith(".js", StringComparison.OrdinalIgnoreCase);

        ctx.Context.Response.Headers.CacheControl = revalidate
            ? "no-cache, must-revalidate"
            : "public, max-age=604800";
    },
});

app.Run();
