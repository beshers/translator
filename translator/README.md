# 🌍 Universal Translator

A professional, modern multilingual translation desktop application built with **C#** and **Avalonia UI**. Supporting real-time translation between multiple languages with a beautiful, responsive user interface.

## ✨ Features

### Core Translation Features
- **Real-time Translation**: Instant translation as you type with intelligent debouncing
- **Multi-language Support**: 10+ languages including:
  - 🇬🇧 English
  - 🇸🇦 Arabic (with RTL support)
  - 🇩🇪 German
  - 🇪🇸 Spanish
  - 🇫🇷 French
  - 🇮🇹 Italian
  - 🇵🇹 Portuguese
  - 🇷🇺 Russian
  - 🇨🇳 Chinese (Simplified)
  - 🇯🇵 Japanese

### User Interface & Experience
- **Modern Design**: Clean, professional UI with intuitive layout
- **Responsive Layout**: Adapts to different window sizes (min 800x500)
- **Dark & Light Mode**: Toggle between themes with system detection
- **RTL Support**: Full Right-to-Left text rendering for Arabic
- **Copy to Clipboard**: Quick copy buttons for source and translated text
- **Swap Languages**: Instantly swap source and target languages with their text
- **Clear Function**: Clear all content with one click

### Technical Excellence
- **MVVM Architecture**: Clean separation of concerns using MVVM pattern
- **Async Operations**: Non-blocking translation with cancellation support
- **Error Handling**: Graceful error handling with user-friendly messages
- **Loading States**: Visual feedback during translation
- **Cross-platform**: Works on Windows, Linux, and macOS

## 🛠️ Tech Stack

- **Language**: C# (.NET 10.0)
- **UI Framework**: Avalonia UI 12.0.1
- **MVVM**: CommunityToolkit.MVVM 8.3.2
- **Translation API**: LibreTranslate (free, open-source)
- **Design Pattern**: MVVM with async/await

## 📦 Project Structure

```
translator/
├── Models/
│   ├── Language.cs              # Language definition with RTL support
│   ├── TranslationResult.cs      # Translation result model
│   └── AppTheme.cs              # Theme enumeration
├── ViewModels/
│   └── MainWindowViewModel.cs    # Main MVVM view model
├── Services/
│   ├── ITranslationService.cs    # Translation service interface
│   ├── TranslationService.cs     # LibreTranslate API integration
│   └── ThemeService.cs           # Theme management
├── Views/
│   ├── MainWindow.axaml          # Main window UI
│   └── MainWindow.axaml.cs       # Code-behind
├── Resources/
│   ├── Themes.axaml              # Color and spacing definitions
│   └── AppStyles.axaml           # Custom control styles
├── Utils/
│   └── ValueConverters.cs        # XAML value converters
├── App.axaml                     # Application root
├── App.axaml.cs                  # Application code-behind
└── Program.cs                    # Entry point
```

## 🚀 Getting Started

### Prerequisites
- .NET 10.0 SDK or later
- Visual Studio Code or Visual Studio 2022+

### Installation

1. **Clone or extract the project**
   ```bash
   cd translator
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Build for release**
   ```bash
   dotnet publish -c Release
   ```

## 💡 How to Use

### Basic Translation
1. **Select Source Language**: Choose the language of the text you want to translate (default: English)
2. **Enter Text**: Type or paste text in the left panel
3. **Select Target Language**: Choose the language you want to translate to (default: Arabic)
4. **View Translation**: The translation appears in real-time in the right panel

### Available Actions
- **Swap (⇄)**: Instantly swap languages and text positions
- **Clear (🗑️)**: Clear all text fields
- **Copy Source (📋)**: Copy the original text to clipboard
- **Copy Translation (📋)**: Copy the translated text to clipboard
- **Toggle Theme (🌙)**: Switch between dark and light modes

## 🔧 Advanced Features

### Real-time Translation with Debouncing
The application uses intelligent debouncing to prevent excessive API calls:
- 300ms delay after you stop typing
- Automatic cancellation of pending requests
- Smooth user experience without overloading the API

### RTL Support for Arabic
- Automatic text direction detection
- Right-aligned text for RTL languages
- Proper character shaping for Arabic text

### Error Handling
- Connection error detection
- Timeout handling
- User-friendly error messages
- Automatic retry capability

### Theme Management
- System theme detection
- Manual dark/light mode toggle
- Persistent theme settings
- Fluent Design System colors

## 🏗️ Architecture Details

### MVVM Pattern
The application follows the Model-View-ViewModel pattern:
- **Model**: `Language`, `TranslationResult` - Data models
- **View**: XAML files with binding expressions
- **ViewModel**: `MainWindowViewModel` - UI logic and state management

### Async/Await Pattern
All heavy operations are asynchronous:
```csharp
// Translation requests don't block the UI
var result = await _translationService.TranslateAsync(...);

// Copy operations use async clipboard
await Avalonia.Application.Current.Clipboard.SetTextAsync(text);
```

### Dependency Injection Ready
Services are designed for easy DI integration:
```csharp
// Currently instantiated in ViewModel, but can be moved to DI container
_translationService = new TranslationService();
_themeService = new ThemeService();
```

## 🌐 Translation Service

The application uses **LibreTranslate**, a free, open-source translation API:
- No API key required
- Runs on public endpoint: `https://api.libretranslate.de`
- Fast and reliable
- Supports 30+ languages
- Self-hosted option available

### Switching Translation Providers
To use a different API (Google Translate, DeepL, etc.):

1. Create a new implementation of `ITranslationService`
2. Update the translation API calls in the new service
3. Inject the new service in `MainWindowViewModel`

## 📝 Code Examples

### Creating a Translation
```csharp
var result = await translationService.TranslateAsync(
    text: "Hello, World!",
    sourceLanguage: "en",
    targetLanguage: "ar"
);

Console.WriteLine(result.TranslatedText); // مرحبا بالعالم
```

### Language Selection
```csharp
var languages = translationService.GetSupportedLanguages();
var arabic = languages.FirstOrDefault(l => l.Code == "ar");
```

### Theme Toggle
```csharp
var themeService = new ThemeService();
themeService.CurrentTheme = AppTheme.Dark;
var variant = themeService.GetThemeVariant();
```

## 🎨 Customization

### Adding New Languages
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

### Styling & Themes
Modify colors in `Resources/Themes.axaml`:
```xml
<Color x:Key="AccentColor">#6200EA</Color>
<Color x:Key="ErrorColor">#B3261E</Color>
```

### Custom Converters
Add new converters in `Utils/ValueConverters.cs` and register them in `App.xaml`

## 🔒 Error Handling

The application handles various error scenarios:
- **Network Errors**: Connection timeouts and failures
- **API Errors**: Invalid responses from translation service
- **Validation Errors**: Empty text or invalid language selection
- **Clipboard Errors**: Permission issues with clipboard access

All errors are displayed to the user in an error panel at the bottom of the window.

## 🚀 Performance Optimizations

1. **Debounced Input**: Delays translation requests to reduce API calls
2. **Request Cancellation**: Cancels pending requests when a new translation is initiated
3. **Lazy Loading**: Languages loaded once during application startup
4. **Async Operations**: All I/O operations are non-blocking
5. **Resource Consolidation**: Shared theme resources and styles

## 📱 Multi-platform Support

The application is built with cross-platform support:
- ✅ Windows
- ✅ Linux (GTK, X11)
- ✅ macOS

No platform-specific code is used. All functionality works identically across platforms.

## 🧪 Testing

The MVVM architecture makes the application testable:
```csharp
// Example unit test for ViewModel
var viewModel = new MainWindowViewModel();
viewModel.SourceText = "Hello";
viewModel.SelectedSourceLanguage = englishLanguage;
viewModel.SelectedTargetLanguage = arabicLanguage;

await viewModel.TranslateTextCommand.ExecuteAsync(null);

Assert.NotEmpty(viewModel.TranslatedText);
```

## 🔄 Future Enhancements

Potential features for future versions:
- **Translation History**: Save and review past translations
- **Favorites**: Mark and save frequently used translations
- **Voice Input**: Speech-to-text for hands-free translation
- **Voice Output**: Text-to-speech pronunciation
- **Auto Language Detection**: Detect source language automatically
- **Offline Mode**: Download language packs for offline translation
- **Settings Panel**: Customizable preferences
- **Dark Mode Scheduling**: Auto-switch based on time of day

## 📖 Documentation

- **Code Comments**: Extensive XML documentation on all public members
- **Property Documentation**: All observable properties documented
- **Service Documentation**: Complete interface and implementation docs
- **Type-safe**: Nullability annotations enabled for better code safety

## 🤝 Contributing

The code is structured for easy contribution:
1. Services are injectable - easy to replace or extend
2. MVVM pattern allows testing without UI
3. Resource files are centralized for easy theming
4. Clear separation of concerns

## 📄 License

This project is provided as-is for educational and commercial use.

## 🐛 Troubleshooting

### Translation Not Working
- Check internet connection
- Verify the translation API is accessible
- Check firewall settings

### UI Not Responsive
- Clear temporary files: `dotnet clean`
- Rebuild project: `dotnet build`
- Restart the application

### Clipboard Issues
- Ensure the application has clipboard permissions
- On Linux, install: `sudo apt-get install xsel xclip`

### RTL Text Not Displaying Correctly
- Verify Arabic text uses proper Unicode characters
- Check font supports Arabic characters
- On Linux, install: `sudo apt-get install fonts-noto-naskh-arabic`

## 📞 Support

For issues or questions:
1. Check the code comments and XML documentation
2. Review the MVVM pattern implementation
3. Examine the service layer design
4. Check Avalonia documentation: https://docs.avaloniaui.net/

## 🎓 Learning Resources

This project demonstrates:
- **MVVM Pattern**: Proper view model implementation
- **Async/Await**: Correct usage for UI responsiveness
- **Avalonia UI**: Modern desktop UI development
- **Data Binding**: XAML binding and converters
- **Service Architecture**: Clean separation of concerns
- **Error Handling**: Graceful error management
- **Theme Management**: Dynamic theme switching

---

**Built with ❤️ using Avalonia UI and .NET**

For the latest information on Avalonia, visit: https://avaloniaui.net/
