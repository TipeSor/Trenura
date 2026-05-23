using System.Reflection;

namespace Engine;

public static class Embedded
{
    public static byte[] ReadBytes(string resourceName, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

        using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
        using MemoryStream ms = new();

        stream.CopyTo(ms);
        return ms.ToArray();
    }

    public static byte[] ReadAsset(string resourceNameOrPath, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        string resourceName = ResolveAssetName(resourceNameOrPath, assembly);
        return ReadBytes(resourceName, assembly);
    }

    public static string ResolveAssetName(string resourceNameOrPath, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        string[] assetNames = assembly.GetManifestResourceNames();

        if (assetNames.Contains(resourceNameOrPath))
            return resourceNameOrPath;

        string normalizedPath = resourceNameOrPath
            .Replace('\\', '.')
            .Replace('/', '.')
            .Trim('.');

        string? resolvedName = assetNames.FirstOrDefault(name =>
            name.EndsWith(normalizedPath, StringComparison.OrdinalIgnoreCase));

        if (resolvedName != null)
            return resolvedName;

        throw new ArgumentException($"Embedded asset not found: {resourceNameOrPath}");
    }

    public static IEnumerable<string> AssetNames(Assembly? assembly = null)
        => (assembly ?? Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetManifestResourceNames();
}
