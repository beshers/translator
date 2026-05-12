using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using translator.Models;

namespace translator.Services;

public sealed class ApiTranslationHistoryService : ITranslationHistoryService
{
    private readonly HttpClient? _httpClient;

    public ApiTranslationHistoryService()
    {
        var apiUrl = Environment.GetEnvironmentVariable("TRANSLATOR_API_URL");

        if (!string.IsNullOrWhiteSpace(apiUrl) &&
            Uri.TryCreate(apiUrl, UriKind.Absolute, out var baseAddress))
        {
            _httpClient = new HttpClient
            {
                BaseAddress = baseAddress,
                Timeout = TimeSpan.FromSeconds(10)
            };
        }
    }

    public async Task SaveAsync(
        string sourceText,
        string translatedText,
        Language sourceLanguage,
        Language targetLanguage,
        CancellationToken cancellationToken = default)
    {
        if (_httpClient is null)
        {
            return;
        }

        var request = new CreateTranslationRecordRequest(
            sourceLanguage.Code,
            targetLanguage.Code,
            sourceText,
            translatedText);

        for (var attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(
                    "api/translations",
                    request,
                    cancellationToken);

                response.EnsureSuccessStatusCode();
                return;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch when (attempt < 3)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(250 * attempt), cancellationToken);
            }
            catch
            {
                return;
            }
        }
    }

    private sealed record CreateTranslationRecordRequest(
        string SourceLanguageCode,
        string TargetLanguageCode,
        string SourceText,
        string TranslatedText);
}
