using Raylib_cs;
using System.IO;

namespace Engine;

/// <summary>
/// Class which manage textures
/// </summary>
public static class TextureManager
{
    private static readonly Dictionary<string, Texture2D> _texture2Ds = [];

    /// <summary>
    /// Get All Textures
    /// </summary>
    public static List<Texture2D> Textures => [.._texture2Ds.Values];

    /// <summary>
    /// Checks if the texture with the specified name exists in the manager.
    /// </summary>
    /// <param name="name">The name of the texture.</param>
    /// <returns>True if the texture exists, otherwise false.</returns>
    public static bool HasTexture(string name) => _texture2Ds.ContainsKey(name);

    ///<summary>
    /// Removes a texture from the manager.
    /// </summary>
    /// <param name="name">The name of the texture.</param>
    /// <exception cref="ArgumentException">Thrown if the texture is not found.</exception>
    public static void RemoveTexture(string name)
    {
        if (_texture2Ds.TryGetValue(name, out var texture))
        {
            Raylib.UnloadTexture(texture);
            _texture2Ds.Remove(name);
            return;
        }

        DebugManager.Log(LogLevel.Error, $"Texture not found: {name}");
        throw new ArgumentException($"Texture not found : {name}");
    }

    /// <summary>
    /// Adds a texture to the manager.
    /// </summary>
    /// <param name="name">The name of the texture.</param>
    /// <param name="texture2D">The texture to add.</param>
    public static void AddTexture(string name, Texture2D texture2D)
    {
        if (!_texture2Ds.TryAdd(name, texture2D))
            DebugManager.Log(
                LogLevel.Warning,
                $"Texture already exists: {name}"
            );
    }

    /// <summary>
    /// Adds a texture to the manager.
    /// </summary>
    /// <param name="name">The name of the texture.</param>
    /// <param name="file">The file path of the texture.</param>
    public static void AddTexture(string name, string file)
    {
        if (!_texture2Ds.TryAdd(name, Raylib.LoadTexture(file)))
            DebugManager.Log(
                LogLevel.Warning,
                $"Texture already exists: {name}"
            );
    }

    /// <summary>
    /// Adds a texture from an embedded resource.
    /// </summary>
    /// <param name="name">The name of the texture.</param>
    /// <param name="resourceNameOrPath">The embedded resource name or asset path.</param>
    public static void AddEmbeddedTexture(string name, string resourceNameOrPath)
    {
        if (_texture2Ds.ContainsKey(name))
        {
            DebugManager.Log(LogLevel.Warning, $"Texture already exists: {name}");
            return;
        }

        byte[] bytes = Embedded.ReadAsset(resourceNameOrPath);
        string extension = GetFileType(resourceNameOrPath);
        Raylib_cs.Image image = Raylib.LoadImageFromMemory(extension, bytes);
        Texture2D texture = Raylib.LoadTextureFromImage(image);
        Raylib.UnloadImage(image);

        _texture2Ds[name] = texture;
    }

    /// <summary>
    /// Gets a texture from the manager.
    /// </summary>
    /// <param name="name">The name of the texture.</param>
    /// <returns>The texture.</returns>
    /// <exception cref="ArgumentException">Thrown if the texture is not found.</exception>
    public static Texture2D GetTexture(string name)
    {
        if (_texture2Ds.TryGetValue(name, out var texture))
            return texture;
        DebugManager.Log(LogLevel.Error, $"Texture not found: {name}");
        throw new ArgumentException($"Texture not found : {name}");
    }

    internal static void Unload()
    {
        foreach (var texture in _texture2Ds.Values)
            Raylib.UnloadTexture(texture);
        _texture2Ds.Clear();
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

        throw new ArgumentException($"Unable to determine file type for embedded texture: {resourceNameOrPath}");
    }
}
