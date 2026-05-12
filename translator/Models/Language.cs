namespace translator.Models;

/// <summary>
/// Represents a supported translation language with its code and display name.
/// </summary>
public class Language
{
    /// <summary>
    /// Gets the language code (e.g., "en", "ar", "de").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets the display name of the language.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the native name of the language (for display to native speakers).
    /// </summary>
    public string NativeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this language uses RTL (Right-to-Left) text direction.
    /// </summary>
    public bool IsRtl { get; set; }

    /// <summary>
    /// Gets the flag emoji for the language.
    /// </summary>
    public string Flag { get; set; } = string.Empty;

    public override string ToString() => Name;

    public override bool Equals(object? obj) => 
        obj is Language lang && lang.Code == Code;

    public override int GetHashCode() => Code.GetHashCode();
}
