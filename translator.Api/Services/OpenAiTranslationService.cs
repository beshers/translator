using OpenAI.Chat;

namespace translator.Api.Services;

public sealed class OpenAiTranslationService : IAiTranslationService
{
    private readonly string? _apiKey;
    private readonly string _model;

    public OpenAiTranslationService(IConfiguration configuration)
    {
        _apiKey = configuration["OPENAI_API_KEY"];
        _model = string.IsNullOrWhiteSpace(configuration["OPENAI_TRANSLATION_MODEL"])
            ? "gpt-5.1"
            : configuration["OPENAI_TRANSLATION_MODEL"]!;
    }

    public async Task<string> TranslateAsync(
        string sourceText,
        string sourceLanguageCode,
        string targetLanguageCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new InvalidOperationException(
                "Missing AI configuration. Set OPENAI_API_KEY in Render environment variables.");
        }

        var chatClient = new ChatClient(_model, _apiKey);
        var messages = new ChatMessage[]
        {
            new SystemChatMessage(
                "You are a precise translation engine. Translate the user's text from the source language code to the target language code. Return only the translated text, without quotes, explanations, markdown, or extra alternatives."),
            new UserChatMessage(
                $"Source language: {sourceLanguageCode}\nTarget language: {targetLanguageCode}\nText:\n{sourceText}")
        };

        var completion = await chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        var translatedText = completion.Value.Content.FirstOrDefault()?.Text?.Trim();

        if (string.IsNullOrWhiteSpace(translatedText))
        {
            throw new InvalidOperationException("The AI translator returned an empty response.");
        }

        return translatedText;
    }
}
