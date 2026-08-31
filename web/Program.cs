using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("Resend", client =>
{
    client.BaseAddress = new Uri("https://api.resend.com/");
    client.Timeout = TimeSpan.FromSeconds(15);
});

// Keep anonymous website-form abuse from flooding the client's inbox or Resend account.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("contact-form", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRateLimiter();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

app.MapPost("/api/contact", async (
    ContactRequest request,
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    ILogger<Program> logger,
    CancellationToken cancellationToken) =>
{
    // Honeypot: real visitors never see or fill this field. Silently accept bots so
    // they do not learn how the filter works.
    if (!string.IsNullOrWhiteSpace(request.Website))
    {
        return Results.Ok(new { success = true, message = "Thanks. Your message has been sent." });
    }

    var name = request.Name?.Trim() ?? string.Empty;
    var phone = request.Phone?.Trim() ?? string.Empty;
    var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
    var equipment = string.IsNullOrWhiteSpace(request.Equipment) ? null : request.Equipment.Trim();
    var message = request.Message?.Trim() ?? string.Empty;

    var errors = new Dictionary<string, string[]>();

    if (name.Length is < 2 or > 100)
        errors["name"] = ["Please enter your name."];

    if (phone.Length is < 7 or > 40)
        errors["phone"] = ["Please enter a valid phone number."];

    if (email is not null && (email.Length > 254 || !MailAddress.TryCreate(email, out _)))
        errors["email"] = ["Please enter a valid email address or leave it blank."];

    if (equipment is not null && equipment.Length > 160)
        errors["equipment"] = ["Vehicle or equipment details must be 160 characters or less."];

    if (message.Length is < 10 or > 4000)
        errors["message"] = ["Please enter a message between 10 and 4,000 characters."];

    if (errors.Count > 0)
        return Results.ValidationProblem(errors);

    var apiKey = configuration["Resend:ApiKey"]
                 ?? Environment.GetEnvironmentVariable("RESEND_API_KEY");
    var fromEmail = configuration["Resend:FromEmail"]
                    ?? Environment.GetEnvironmentVariable("RESEND_FROM_EMAIL");
    var toEmail = configuration["Contact:ToEmail"]
                  ?? Environment.GetEnvironmentVariable("CONTACT_TO_EMAIL");

    if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(toEmail))
    {
        logger.LogError("Contact form is not configured. Resend API key, sender, or recipient is missing.");
        return Results.Json(
            new { success = false, message = "The contact form is temporarily unavailable. Please call (985) 868-1438." },
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    Func<string, string> encode = HtmlEncoder.Default.Encode;
    var safeName = encode(name);
    var safePhone = encode(phone);
    var safeEmail = email is null ? "Not provided" : encode(email);
    var safeEquipment = equipment is null ? "Not provided" : encode(equipment);
    var safeMessage = encode(message).Replace("\r\n", "<br>").Replace("\n", "<br>");

    var subjectName = name.Replace("\r", " ").Replace("\n", " ");
    var subject = $"Pro Diesel Worx website inquiry — {subjectName}";

    var html = $$"""
        <!doctype html>
        <html>
        <body style="margin:0;background:#f3f3f2;font-family:Arial,Helvetica,sans-serif;color:#17191b;">
          <div style="max-width:680px;margin:0 auto;padding:32px 18px;">
            <div style="background:#17191b;color:#fff;padding:24px 28px;border-top:5px solid #e01922;">
              <div style="font-size:12px;letter-spacing:1.8px;text-transform:uppercase;color:#ff7075;font-weight:700;">Pro Diesel Worx</div>
              <h1 style="margin:8px 0 0;font-size:26px;line-height:1.2;">New Website Inquiry</h1>
            </div>
            <div style="background:#fff;padding:28px;border:1px solid #dedfdf;border-top:0;">
              <table role="presentation" style="width:100%;border-collapse:collapse;font-size:15px;line-height:1.5;">
                <tr><td style="padding:8px 0;width:155px;color:#6a6f73;font-weight:700;">Name</td><td style="padding:8px 0;">{{safeName}}</td></tr>
                <tr><td style="padding:8px 0;color:#6a6f73;font-weight:700;">Phone</td><td style="padding:8px 0;"><a href="tel:{{WebUtility.HtmlEncode(phone)}}" style="color:#c81018;">{{safePhone}}</a></td></tr>
                <tr><td style="padding:8px 0;color:#6a6f73;font-weight:700;">Email</td><td style="padding:8px 0;">{{safeEmail}}</td></tr>
                <tr><td style="padding:8px 0;color:#6a6f73;font-weight:700;">Vehicle / equipment</td><td style="padding:8px 0;">{{safeEquipment}}</td></tr>
              </table>
              <div style="margin-top:22px;padding-top:22px;border-top:1px solid #e5e5e5;">
                <div style="font-size:12px;letter-spacing:1.3px;text-transform:uppercase;color:#6a6f73;font-weight:700;margin-bottom:8px;">Customer message</div>
                <div style="font-size:16px;line-height:1.65;">{{safeMessage}}</div>
              </div>
            </div>
            <div style="padding:14px 2px;color:#777;font-size:12px;">Submitted from prodieselworx.com</div>
          </div>
        </body>
        </html>
        """;

    var text = $"""
        New Pro Diesel Worx website inquiry

        Name: {name}
        Phone: {phone}
        Email: {email ?? "Not provided"}
        Vehicle / equipment: {equipment ?? "Not provided"}

        Message:
        {message}

        Submitted from prodieselworx.com
        """;

    var payload = new Dictionary<string, object?>
    {
        ["from"] = fromEmail,
        ["to"] = new[] { toEmail },
        ["subject"] = subject,
        ["html"] = html,
        ["text"] = text,
        ["tags"] = new[] { new { name = "source", value = "website_contact" } }
    };

    // Reply in the client's email app and the response goes directly to the visitor.
    if (email is not null)
        payload["reply_to"] = email;

    try
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "emails")
        {
            Content = JsonContent.Create(payload)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var client = httpClientFactory.CreateClient("Resend");
        using var response = await client.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Resend rejected contact-form email with status {StatusCode}: {ResponseBody}",
                (int)response.StatusCode, responseBody);

            return Results.Json(
                new { success = false, message = "We couldn't send your message right now. Please call (985) 868-1438." },
                statusCode: StatusCodes.Status502BadGateway);
        }

        return Results.Ok(new
        {
            success = true,
            message = "Thanks — your message was sent to Pro Diesel Worx. We'll be in touch as soon as possible."
        });
    }
    catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
    {
        logger.LogError("Resend timed out while sending a contact-form submission.");
        return Results.Json(
            new { success = false, message = "The form timed out. Please try again or call (985) 868-1438." },
            statusCode: StatusCodes.Status504GatewayTimeout);
    }
    catch (HttpRequestException ex)
    {
        logger.LogError(ex, "Unable to reach Resend while sending a contact-form submission.");
        return Results.Json(
            new { success = false, message = "We couldn't send your message right now. Please call (985) 868-1438." },
            statusCode: StatusCodes.Status502BadGateway);
    }
}).RequireRateLimiting("contact-form");

// The pages carry no fingerprint in their URLs, so make HTML/CSS/JS revalidate every
// time (ETags turn that into a cheap 304) and let images cache properly.
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

internal sealed record ContactRequest(
    string? Name,
    string? Phone,
    string? Email,
    string? Equipment,
    string? Message,
    string? Website);
