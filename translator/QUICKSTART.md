# 🚀 Quick Start Guide - Universal Translator

## Getting Started

### Prerequisites
- .NET 10.0 SDK or later
- Visual Studio Code or any text editor
- Windows, Linux, or macOS

### Installation & Running

1. **Navigate to the project directory**
   ```bash
   cd translator
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Build for production**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

## Using the Application

### Translation
1. **Select Source Language**: Choose the language you're translating FROM (default: English)
2. **Type or Paste Text**: Enter text in the left panel
3. **Select Target Language**: Choose the language you're translating TO (default: Arabic)
4. **View Translation**: Translation appears automatically in the right panel

### Available Actions
- **⇄ Swap**: Swap languages and text positions
- **🗑️ Clear**: Clear all text fields
- **📋 Copy**: Copy text to clipboard
- **🌙 Theme Toggle**: Switch between dark and light modes

### Features
✅ Real-time translation with debouncing (300ms delay)
✅ 10+ supported languages
✅ RTL support for Arabic text
✅ Dark and Light mode
✅ Error handling and loading indicators
✅ Responsive, modern UI
✅ No API key required (uses LibreTranslate free API)

## Project Architecture

```
translator/
├── Models/                  # Data models (Language, TranslationResult)
├── ViewModels/             # MVVM view models (MainWindowViewModel)
├── Services/               # Business logic services
│   ├── TranslationService  # API integration
│   ├── ThemeService        # Theme management
│   └── ClipboardService    # Clipboard operations
├── Views/                  # XAML UI files (MainWindow)
├── Resources/              # Styles and themes
├── Utils/                  # Value converters and helpers
└── App.axaml               # Application root
```

## Supported Languages

- 🇬🇧 English
- 🇸🇦 Arabic (RTL)
- 🇩🇪 German
- 🇪🇸 Spanish
- 🇫🇷 French
- 🇮🇹 Italian
- 🇵🇹 Portuguese
- 🇷🇺 Russian
- 🇨🇳 Chinese (Simplified)
- 🇯🇵 Japanese

## MVVM Architecture

The application follows the Model-View-ViewModel pattern:

- **Model**: `Language`, `TranslationResult` - Pure data models
- **View**: XAML files with data bindings
- **ViewModel**: `MainWindowViewModel` - Handles all UI logic and state

## Commands Available

- `TranslateTextCommand`: Performs translation
- `SwapLanguagesCommand`: Swaps languages
- `ClearSourceTextCommand`: Clears text
- `CopyTranslatedTextCommand`: Copies translation
- `CopySourceTextCommand`: Copies source
- `ToggleThemeCommand`: Toggles dark/light mode

## Troubleshooting

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

### Runtime Issues
- Ensure you have internet connection (for translation API)
- Check firewall settings
- Restart the application

### Text Display Issues (Arabic)
- Ensure proper Unicode support
- On Linux: Install fonts with `sudo apt-get install fonts-noto-naskh-arabic`

## Technical Details

- **Language**: C# 13 (with nullable reference types enabled)
- **Framework**: .NET 10.0
- **UI**: Avalonia 12.0.1
- **MVVM**: CommunityToolkit.MVVM 8.3.2
- **Translation API**: LibreTranslate (free, open-source)
- **Async Model**: Full async/await support
- **Threading**: Non-blocking UI operations

## Code Quality

✓ Comprehensive XML documentation
✓ Async/await best practices
✓ Error handling
✓ MVVM pattern compliance
✓ Dependency injection ready
✓ Testable architecture

## Multi-Platform Support

✅ Windows - Full support
✅ Linux (GTK/X11) - Full support
✅ macOS - Full support

No platform-specific code required - everything is cross-platform!

## Extending the Application

### Add New Language
Edit `TranslationService.InitializeSupportedLanguages()`:
```csharp
new Language
{
    Code = "ko",
    Name = "Korean",
    NativeName = "한국어",
    IsRtl = false,
    Flag = "🇰🇷"
}
```

### Add New Translation Provider
1. Implement `ITranslationService` interface
2. Update the service instantiation in `MainWindowViewModel`
3. Handle API responses appropriately

### Customize Styles
Edit `Resources/AppStyles.axaml` and `Resources/Themes.axaml`

## Performance Tips

- The app uses debouncing (300ms) to reduce API calls
- Cancellation tokens prevent duplicate requests
- All operations are non-blocking
- Minimal memory footprint

## Future Enhancements

Potential features to add:
- Translation history
- Favorite translations  
- Voice input/output (speech-to-text, text-to-speech)
- Auto language detection
- Offline translation support
- Settings panel
- Translation statistics

---

**Enjoy your professional translation application! 🎉**

For more information, see README.md
