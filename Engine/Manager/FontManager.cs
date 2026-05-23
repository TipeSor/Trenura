using Raylib_cs;
using System.IO;

namespace Engine;

/// <summary>
/// Class which manage fonts
/// </summary>
public static class FontManager
{
    private static readonly Dictionary<string, Font> _fonts = [];

    /// <summary>
    /// Get all fonts
    /// </summary>
    public static List<Font> Fonts => [.._fonts.Values];

    /// <summary>
    /// Checks if the font with the specified name exists in the manager
    /// </summary>
    /// <param name="name">The name of the font</param>
    /// <returns>True if the font exists, otherwise false</returns>
    public static bool HasFont(string name) => _fonts.ContainsKey(name);

    /// <summary>
    /// Removes a font from the manager
    /// </summary>
    /// <param name="name">The name of the font</param>
    /// <exception cref="ArgumentException">Thrown if the font is not found</exception>
    public static void RemoveFont(string name)
    {
        if (_fonts.TryGetValue(name, out Font font))
        {
            Raylib.UnloadFont(font);
            _fonts.Remove(name);
            return;
        }

        DebugManager.Log(LogLevel.Error, $"Font not found: {name}");
        throw new ArgumentException($"Font not found : {name}");
    }

    /// <summary>
    /// Adds a font to the manager
    /// </summary>
    /// <param name="name">The name of the font</param>
    /// <param name="font">The font to add</param>
    public static void AddFont(string name, Font font)
    {
        if (!_fonts.TryAdd(name, font))
            DebugManager.Log(
                LogLevel.Warning,
                $"Font already exists: {name}"
            );
    }

    /// <summary>
    /// Adds a font to the manager
    /// </summary>
    /// <param name="name">The name of the font</param>
    /// <param name="file">The file path of the font</param>
    public static void AddFont(string name, string file)
    {
        if (!_fonts.TryAdd(name, Raylib.LoadFont(file)))
            DebugManager.Log(
                LogLevel.Warning,
                $"Font already exists: {name}"
            );
    }

    /// <summary>
    /// Adds a font from an embedded resource.
    /// </summary>
    /// <param name="name">The name of the font.</param>
    /// <param name="resourceNameOrPath">The embedded resource name or asset path.</param>
    /// <param name="fontSize">The base font size to load.</param>
    /// <param name="codepoints">Optional codepoints to load.</param>
    public static void AddEmbeddedFont(string name, string resourceNameOrPath, int fontSize, int[]? codepoints = null)
    {
        if (_fonts.ContainsKey(name))
        {
            DebugManager.Log(LogLevel.Warning, $"Font already exists: {name}");
            return;
        }

        byte[] bytes = Embedded.ReadAsset(resourceNameOrPath);
        string extension = GetFileType(resourceNameOrPath);
        Font font = Raylib.LoadFontFromMemory(extension, bytes, fontSize, codepoints, codepoints?.Length ?? 0);
        _fonts[name] = font;
    }

    /// <summary>
    /// Gets a font from the manager
    /// </summary>
    /// <param name="name">The name of the font</param>
    /// <returns>The font</returns>
    /// <exception cref="ArgumentException">Thrown if the font is not found</exception>
    public static Font GetFont(string name)
    {
        if (_fonts.TryGetValue(name, out Font font))
            return font;

        DebugManager.Log(LogLevel.Error, $"Font not found: {name}");
        throw new ArgumentException($"Font not found : {name}");
    }

    internal static void Unload()
    {
        foreach (Font font in _fonts.Values)
            Raylib.UnloadFont(font);

        _fonts.Clear();
    }

    private static string GetFileType(string resourceNameOrPath)
    {
        string extension = Path.GetExtension(resourceNameOrPath);
        if (!string.IsNullOrEmpty(extension))
            return extension;

        string resourceName = Embedded.ResolveAssetName(resourceNameOrPath);
        extension = Path.GetExtension(resourceName);
        if (!string.IsNullOrEmpty(extension))
            return extension;

        throw new ArgumentException($"Unable to determine file type for embedded font: {resourceNameOrPath}");
    }
}
