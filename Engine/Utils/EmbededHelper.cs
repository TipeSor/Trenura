using System.Reflection;

namespace Engine;

public static class Embedded
{
    public static byte[] ReadBytes(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
        using MemoryStream ms = new();

        stream.CopyTo(ms);
        return ms.ToArray();
    }

    public static IEnumerable<string> AssetNames()
        => Assembly.GetEntryAssembly()!.GetManifestResourceNames();
}
