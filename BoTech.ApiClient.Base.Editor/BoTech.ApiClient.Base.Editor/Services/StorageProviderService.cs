using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using BoTech.ApiClient.Base.Editor.Services.AvaloniaTopLevel;

namespace BoTech.ApiClient.Base.Editor.Services;

public static class StorageProviderService
{
    public static IStorageProvider GetStorageProvider()
    {
        return TopLevelProvider.Instance.GetTopLevel().StorageProvider;
    }
}