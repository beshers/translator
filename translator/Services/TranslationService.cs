using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using translator.Models;

namespace translator.Services;

/// <summary>
/// Translation service. Uses the cloud API when TRANSLATOR_API_URL is set, otherwise falls back to LibreTranslate.
/// </summary>
public class TranslationService : ITranslationService
{
    private readonly HttpClient _httpClient;
    private readonly bool _useCloudApi;
    private const string ApiBaseUrl = "https://api.libretranslate.de";

    private static readonly Lazy<IReadOnlyList<Language>> SupportedLanguages =
        new(InitializeSupportedLanguages);

    public TranslationService()
    {
        var cloudApiUrl = Environment.GetEnvironmentVariable("TRANSLATOR_API_URL");

        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        if (!string.IsNullOrWhiteSpace(cloudApiUrl) &&
            Uri.TryCreate(cloudApiUrl, UriKind.Absolute, out var cloudApiBaseAddress))
        {
            _httpClient.BaseAddress = cloudApiBaseAddress;
            _useCloudApi = true;
        }
    }

    public async Task<TranslationResult> TranslateAsync(
        string text,
        string sourceLanguage,
        string targetLanguage,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text to translate cannot be empty.", nameof(text));
        }

        try
        {
            if (_useCloudApi)
            {
                return await TranslateWithCloudApiAsync(text, sourceLanguage, targetLanguage, cancellationToken);
            }

            var request = new LibreTranslateRequest
            {
                Q = text,
                Source = sourceLanguage,
                Target = targetLanguage,
                Format = "text"
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"{ApiBaseUrl}/translate",
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<LibreTranslateResponse>(jsonContent) ?? new LibreTranslateResponse();

            return new TranslationResult
            {
                TranslatedText = result.TranslatedText ?? string.Empty,
                SourceLanguage = sourceLanguage,
                TargetLanguage = targetLanguage,
                OriginalText = text,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("Translation service is currently unavailable. Please check your internet connection.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new InvalidOperationException("Translation request timed out. Please try again.", ex);
        }
    }

    public IReadOnlyList<Language> GetSupportedLanguages() => SupportedLanguages.Value;

    private async Task<TranslationResult> TranslateWithCloudApiAsync(
        string text,
        string sourceLanguage,
        string targetLanguage,
        CancellationToken cancellationToken)
    {
        var request = new CloudTranslateRequest(sourceLanguage, targetLanguage, text);
        var response = await _httpClient.PostAsJsonAsync("api/translate", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CloudTranslateResponse>(cancellationToken);

        return new TranslationResult
        {
            TranslatedText = result?.TranslatedText ?? string.Empty,
            SourceLanguage = sourceLanguage,
            TargetLanguage = targetLanguage,
            OriginalText = text,
            Timestamp = DateTime.UtcNow
        };
    }

    private static IReadOnlyList<Language> InitializeSupportedLanguages()
    {
        return new List<Language>
        {
            new() { Code = "en", Name = "English", NativeName = "English", IsRtl = false, Flag = "GB" },
            new() { Code = "ar", Name = "Arabic", NativeName = "العربية", IsRtl = true, Flag = "SA" },
            new() { Code = "de", Name = "German", NativeName = "Deutsch", IsRtl = false, Flag = "DE" },
            new() { Code = "es", Name = "Spanish", NativeName = "Español", IsRtl = false, Flag = "ES" },
            new() { Code = "fr", Name = "French", NativeName = "Français", IsRtl = false, Flag = "FR" },
            new() { Code = "it", Name = "Italian", NativeName = "Italiano", IsRtl = false, Flag = "IT" },
            new() { Code = "pt", Name = "Portuguese", NativeName = "Português", IsRtl = false, Flag = "PT" },
            new() { Code = "ru", Name = "Russian", NativeName = "Русский", IsRtl = false, Flag = "RU" },
            new() { Code = "zh", Name = "Chinese (Simplified)", NativeName = "简体中文", IsRtl = false, Flag = "CN" },
            new() { Code = "ja", Name = "Japanese", NativeName = "日本語", IsRtl = false, Flag = "JP" }
        }.AsReadOnly();
    }

    private class LibreTranslateRequest
    {
        [JsonPropertyName("q")]
        public string Q { get; set; } = string.Empty;

        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("target")]
        public string Target { get; set; } = string.Empty;

        [JsonPropertyName("format")]
        public string Format { get; set; } = "text";
    }

    private class LibreTranslateResponse
    {
        [JsonPropertyName("translatedText")]
        public string? TranslatedText { get; set; }
    }

    private sealed record CloudTranslateRequest(
        string SourceLanguageCode,
        string TargetLanguageCode,
        string SourceText);

    private sealed record CloudTranslateResponse(
        string OriginalText,
        string TranslatedText,
        string SourceLanguage,
        string TargetLanguage,
        string Source);
}
