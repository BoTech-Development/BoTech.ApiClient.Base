using System.Reflection;

namespace BoTech.ApiClient.Base.Services;
/// <summary>
/// This class loads the contents of files located in the executing assembly.
/// </summary>
public static class ResourceFileLoader
{
    /// <summary>
    /// Loads the contents of the given file name (including namespace)
    /// </summary>
    /// <param name="resourceName">The name of the resource including the namespace</param>
    /// <returns>The contents of the file</returns>
    /// <exception cref="ArgumentException"></exception>
    public static string LoadFile(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null) throw new ArgumentException($"Resource '{resourceName}' not found.");
        using StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}