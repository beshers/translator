namespace translator.Api.Models;

public sealed class TranslationRecord
{
    public int Id { get; set; }

    public string SourceLanguageCode { get; set; } = string.Empty;

    public string TargetLanguageCode { get; set; } = string.Empty;

    public string SourceText { get; set; } = string.Empty;

    public string TranslatedText { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }
}
