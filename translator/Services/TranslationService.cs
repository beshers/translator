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
/// Translation service implementation using LibreTranslate API.
/// </summary>
public class TranslationService : ITranslationService
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "https://api.libretranslate.de";

    private static readonly Lazy<IReadOnlyList<Language>> SupportedLanguages =
        new(InitializeSupportedLanguages);

    /// <summary>
    /// Initializes a new instance of the TranslationService class.
    /// </summary>
    public TranslationService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    /// <summary>
    /// Translates text from source language to target language asynchronously.
    /// </summary>
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
            var request = new TranslateRequest
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
            var result = JsonSerializer.Deserialize<TranslateResponse>(jsonContent) ?? new TranslateResponse();

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

    /// <summary>
    /// Gets the list of supported languages.
    /// </summary>
    public IReadOnlyList<Language> GetSupportedLanguages() => SupportedLanguages.Value;

    /// <summary>
    /// Initializes the list of supported languages.
    /// </summary>
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

    /// <summary>
    /// Request model for LibreTranslate API.
    /// </summary>
    private class TranslateRequest
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

    /// <summary>
    /// Response model from LibreTranslate API.
    /// </summary>
    private class TranslateResponse
    {
        [JsonPropertyName("translatedText")]
        public string? TranslatedText { get; set; }
    }
}
