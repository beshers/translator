using Microsoft.EntityFrameworkCore;
using translator.Api.Data;
using translator.Api.Models;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var connectionString = builder.Configuration.GetConnectionString("AivenPostgres");

if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = builder.Configuration["AIVEN_POSTGRES"];
}

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Missing database connection. Set AIVEN_POSTGRES to your Aiven PostgreSQL connection string.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("TranslatorClients", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5000",
                "https://localhost:5001",
                "http://localhost:5173",
                "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("TranslatorClients");

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "translator.Api",
    timestamp = DateTimeOffset.UtcNow
}));

app.MapGet("/api/translations", async (AppDbContext db, CancellationToken cancellationToken) =>
{
    var records = await db.TranslationRecords
        .AsNoTracking()
        .OrderByDescending(record => record.CreatedAtUtc)
        .Take(50)
        .ToListAsync(cancellationToken);

    return Results.Ok(records);
});

app.MapPost("/api/translations", async (
    CreateTranslationRecordRequest request,
    AppDbContext db,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.SourceText) ||
        string.IsNullOrWhiteSpace(request.TranslatedText) ||
        string.IsNullOrWhiteSpace(request.SourceLanguageCode) ||
        string.IsNullOrWhiteSpace(request.TargetLanguageCode))
    {
        return Results.BadRequest("Source text, translated text, and language codes are required.");
    }

    var record = new TranslationRecord
    {
        SourceLanguageCode = request.SourceLanguageCode.Trim(),
        TargetLanguageCode = request.TargetLanguageCode.Trim(),
        SourceText = request.SourceText.Trim(),
        TranslatedText = request.TranslatedText.Trim(),
        CreatedAtUtc = DateTimeOffset.UtcNow
    };

    db.TranslationRecords.Add(record);
    await db.SaveChangesAsync(cancellationToken);

    return Results.Created($"/api/translations/{record.Id}", record);
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.Run();

public sealed record CreateTranslationRecordRequest(
    string SourceLanguageCode,
    string TargetLanguageCode,
    string SourceText,
    string TranslatedText);
