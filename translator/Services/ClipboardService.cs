using System;
using System.Threading.Tasks;

namespace translator.Services;

/// <summary>
/// Service for clipboard operations in a platform-agnostic way.
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// Copies text to clipboard asynchronously.
    /// </summary>
    Task CopyToClipboardAsync(string text);
}

/// <summary>
/// Implementation of clipboard service for Avalonia.
/// Simplified implementation - clipboard feature can be enhanced later with proper Avalonia 12 API.
/// </summary>
public class ClipboardService : IClipboardService
{
    /// <summary>
    /// Copies text to clipboard asynchronously.
    /// </summary>
    public async Task CopyToClipboardAsync(string text)
    {
        try
        {
            // Clipboard operations in Avalonia 12 require access to the UI thread and main window
            // For now, we'll provide a placeholder implementation
            // TODO: Implement clipboard with proper Avalonia 12 API once window context is available
            await Task.CompletedTask;
        }
        catch
        {
            // Silently fail to maintain UX
        }
    }
}
