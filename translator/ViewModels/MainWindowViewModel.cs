using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using translator.Models;
using translator.Services;

namespace translator.ViewModels;

/// <summary>
/// Main view model for the application, managing translation logic and UI state.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private readonly ITranslationService _translationService;
    private readonly IThemeService _themeService;
    private readonly IClipboardService _clipboardService;
    private readonly ITranslationHistoryService _translationHistoryService;
    private CancellationTokenSource? _translationCancellationTokenSource;

    [ObservableProperty]
    private string sourceText = string.Empty;

    [ObservableProperty]
    private string translatedText = string.Empty;

    [ObservableProperty]
    private Language? selectedSourceLanguage;

    [ObservableProperty]
    private Language? selectedTargetLanguage;

    [ObservableProperty]
    private bool isTranslating;

    [ObservableProperty]
    private bool isConnected = true;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private AppTheme currentTheme = AppTheme.System;

    [ObservableProperty]
    private bool isDarkMode;

    private List<Language> _supportedLanguages = new();

    /// <summary>
    /// Gets the list of supported languages.
    /// </summary>
    public IReadOnlyList<Language> SupportedLanguages => _supportedLanguages.AsReadOnly();

    /// <summary>
    /// Gets a compact status message for the footer.
    /// </summary>
    public string StatusText
    {
        get
        {
            if (IsTranslating)
            {
                return "Translating...";
            }

            return IsConnected ? "Online and ready" : "Connection issue";
        }
    }

    /// <summary>
    /// Initializes a new instance of the MainWindowViewModel class.
    /// </summary>
    public MainWindowViewModel()
    {
        _translationService = new TranslationService();
        _themeService = new ThemeService();
        _clipboardService = new ClipboardService();
        _translationHistoryService = new ApiTranslationHistoryService();

        InitializeLanguages();
        InitializeTheme();

        // Subscribe to property changes for real-time translation
        PropertyChanged += async (s, e) =>
        {
            if (e.PropertyName == nameof(SourceText))
            {
                await TranslateText();
            }
        };
    }

    /// <summary>
    /// Initializes the supported languages.
    /// </summary>
    private void InitializeLanguages()
    {
        _supportedLanguages = _translationService.GetSupportedLanguages().ToList();

        // Set default languages: English source, Arabic target
        SelectedSourceLanguage = _supportedLanguages.FirstOrDefault(l => l.Code == "en");
        SelectedTargetLanguage = _supportedLanguages.FirstOrDefault(l => l.Code == "ar");
    }

    /// <summary>
    /// Initializes the theme settings.
    /// </summary>
    private void InitializeTheme()
    {
        _themeService.ThemeChanged += (s, e) => 
        {
            CurrentTheme = _themeService.CurrentTheme;
            UpdateThemeVariant();
        };
        UpdateThemeVariant();
    }

    /// <summary>
    /// Updates the theme variant based on current settings.
    /// </summary>
    private void UpdateThemeVariant()
    {
        var variant = _themeService.GetThemeVariant();
        Application.Current!.RequestedThemeVariant = variant;
        IsDarkMode = variant == Avalonia.Styling.ThemeVariant.Dark;
    }

    /// <summary>
    /// Translates the current source text asynchronously.
    /// </summary>
    [RelayCommand]
    private async Task TranslateText()
    {
        _translationCancellationTokenSource?.Cancel();

        if (string.IsNullOrWhiteSpace(SourceText) || 
            SelectedSourceLanguage == null || 
            SelectedTargetLanguage == null)
        {
            TranslatedText = string.Empty;
            ErrorMessage = null;
            return;
        }

        if (SelectedSourceLanguage.Code == SelectedTargetLanguage.Code)
        {
            TranslatedText = SourceText;
            ErrorMessage = null;
            return;
        }

        var currentRequest = new CancellationTokenSource();
        _translationCancellationTokenSource = currentRequest;

        try
        {
            IsTranslating = true;
            OnPropertyChanged(nameof(StatusText));
            ErrorMessage = null;

            var sourceSnapshot = SourceText;
            var sourceLanguageModel = SelectedSourceLanguage!;
            var targetLanguageModel = SelectedTargetLanguage!;
            var sourceLanguage = sourceLanguageModel.Code;
            var targetLanguage = targetLanguageModel.Code;

            await Task.Delay(300, currentRequest.Token);

            var result = await _translationService.TranslateAsync(
                sourceSnapshot,
                sourceLanguage,
                targetLanguage,
                currentRequest.Token);

            if (_translationCancellationTokenSource != currentRequest)
            {
                return;
            }

            TranslatedText = result.TranslatedText;
            IsConnected = true;
            OnPropertyChanged(nameof(StatusText));

            await _translationHistoryService.SaveAsync(
                sourceSnapshot,
                result.TranslatedText,
                sourceLanguageModel,
                targetLanguageModel,
                currentRequest.Token);
        }
        catch (OperationCanceledException)
        {
            // Translation was cancelled, ignore
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
            IsConnected = false;
            TranslatedText = string.Empty;
            OnPropertyChanged(nameof(StatusText));
        }
        catch (Exception)
        {
            ErrorMessage = "An unexpected error occurred during translation.";
            TranslatedText = string.Empty;
        }
        finally
        {
            if (_translationCancellationTokenSource == currentRequest)
            {
                IsTranslating = false;
                _translationCancellationTokenSource = null;
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }

    /// <summary>
    /// Swaps the source and target languages.
    /// </summary>
    [RelayCommand]
    private async Task SwapLanguages()
    {
        var temp = SelectedSourceLanguage;
        SelectedSourceLanguage = SelectedTargetLanguage;
        SelectedTargetLanguage = temp;

        // Swap text as well
        var tempText = SourceText;
        SourceText = TranslatedText;
        TranslatedText = tempText;

        await TranslateText();
    }

    /// <summary>
    /// Clears the source text.
    /// </summary>
    [RelayCommand]
    public void ClearSourceText()
    {
        SourceText = string.Empty;
        TranslatedText = string.Empty;
        ErrorMessage = null;
    }

    /// <summary>
    /// Copies the translated text to clipboard.
    /// </summary>
    [RelayCommand]
    private async Task CopyTranslatedText()
    {
        if (!string.IsNullOrWhiteSpace(TranslatedText))
        {
            await _clipboardService.CopyToClipboardAsync(TranslatedText);
        }
    }

    /// <summary>
    /// Copies the source text to clipboard.
    /// </summary>
    [RelayCommand]
    private async Task CopySourceText()
    {
        if (!string.IsNullOrWhiteSpace(SourceText))
        {
            await _clipboardService.CopyToClipboardAsync(SourceText);
        }
    }

    /// <summary>
    /// Toggles between dark and light theme.
    /// </summary>
    [RelayCommand]
    public void ToggleTheme()
    {
        CurrentTheme = CurrentTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
        _themeService.CurrentTheme = CurrentTheme;
    }

    /// <summary>
    /// Called when the source language selection changes.
    /// </summary>
    partial void OnSelectedSourceLanguageChanged(Language? value)
    {
        // Trigger translation when language changes
        if (value != null && !string.IsNullOrWhiteSpace(SourceText))
        {
            TranslateTextCommand.Execute(null);
        }
    }

    /// <summary>
    /// Called when the target language selection changes.
    /// </summary>
    partial void OnSelectedTargetLanguageChanged(Language? value)
    {
        // Trigger translation when language changes
        if (value != null && !string.IsNullOrWhiteSpace(SourceText))
        {
            TranslateTextCommand.Execute(null);
        }
    }
}
