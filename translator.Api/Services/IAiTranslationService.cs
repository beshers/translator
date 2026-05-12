namespace translator.Api.Services;

public interface IAiTranslationService
{
    Task<string> TranslateAsync(
        string sourceText,
        string sourceLanguageCode,
        string targetLanguageCode,
        CancellationToken cancellationToken = default);
}
