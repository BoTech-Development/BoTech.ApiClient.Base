using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using BoTech.ApiClient.Base.Editor.Services.AvaloniaTopLevel;

namespace BoTech.ApiClient.Base.Editor.Services.Url;

public static class UrlLauncher
{
    public static async Task<bool> OpenAsync(Uri uri)
    {
        TopLevel topLevel = TopLevelProvider.Instance.GetTopLevel();
        return await topLevel.Launcher.LaunchUriAsync(uri);
    }
}