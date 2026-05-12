using System;
using Avalonia.Styling;
using translator.Models;

namespace translator.Services;

/// <summary>
/// Service for managing application theme settings.
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Gets or sets the current theme.
    /// </summary>
    AppTheme CurrentTheme { get; set; }

    /// <summary>
    /// Gets the Avalonia theme variant based on current settings.
    /// </summary>
    ThemeVariant GetThemeVariant();

    /// <summary>
    /// Occurs when the theme changes.
    /// </summary>
    event EventHandler<EventArgs>? ThemeChanged;
}

/// <summary>
/// Implementation of the theme service.
/// </summary>
public class ThemeService : IThemeService
{
    private AppTheme _currentTheme = AppTheme.System;

    /// <summary>
    /// Gets or sets the current theme.
    /// </summary>
    public AppTheme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                ThemeChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets the Avalonia theme variant based on current settings.
    /// </summary>
    public ThemeVariant GetThemeVariant()
    {
        return CurrentTheme switch
        {
            AppTheme.Light => ThemeVariant.Light,
            AppTheme.Dark => ThemeVariant.Dark,
            AppTheme.System => ThemeVariant.Default,
            _ => ThemeVariant.Default
        };
    }

    /// <summary>
    /// Occurs when the theme changes.
    /// </summary>
    public event EventHandler<EventArgs>? ThemeChanged;
}
