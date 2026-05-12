using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using translator.Models;

namespace translator.Services;

/// <summary>
/// Interface for translation service implementations.
/// </summary>
public interface ITranslationService
{
    /// <summary>
    /// Translates text from source language to target language.
    /// </summary>
    /// <param name="text">The text to translate.</param>
    /// <param name="sourceLanguage">The source language code.</param>
    /// <param name="targetLanguage">The target language code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The translation result.</returns>
    Task<TranslationResult> TranslateAsync(string text, string sourceLanguage, string targetLanguage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of supported languages.
    /// </summary>
    /// <returns>Collection of supported languages.</returns>
    IReadOnlyList<Language> GetSupportedLanguages();
}
