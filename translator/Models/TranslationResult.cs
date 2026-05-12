using System;

namespace translator.Models;

/// <summary>
/// Represents the result of a translation operation.
/// </summary>
public class TranslationResult
{
    /// <summary>
    /// Gets the translated text.
    /// </summary>
    public string TranslatedText { get; set; } = string.Empty;

    /// <summary>
    /// Gets the source language of the translation.
    /// </summary>
    public string SourceLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Gets the target language of the translation.
    /// </summary>
    public string TargetLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Gets the original text that was translated.
    /// </summary>
    public string OriginalText { get; set; } = string.Empty;

    /// <summary>
    /// Gets the timestamp when the translation was performed.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
