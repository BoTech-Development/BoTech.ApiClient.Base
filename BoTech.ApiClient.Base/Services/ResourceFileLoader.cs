using System.Reflection;

namespace BoTech.ApiClient.Base.Services;

public static class ResourceFileLoader
{
    public static string LoadFile(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if(stream == null) throw new ArgumentException($"Resource '{resourceName}' not found.");
        using StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

}