using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace BoTech.ApiClient.Base.Editor.Services.AvaloniaTopLevel;

public class TopLevelProvider
{
    public static TopLevelProvider Instance
    {
        get
        {
            if(field is null)
                throw new InvalidOperationException("Please init first before accessing the instance.");
            return field;
        }
        private set;
    }

    private Visual _topLevelControl;
    private TopLevel? _topLevel;
    private bool _alreadyFetchedTopLevelControl = false;

    private TopLevelProvider(Visual topLevelControl)
    {
        _topLevelControl = topLevelControl;
    }
    public static void CreateInstance(Visual topLevelControl)
    {
        Instance = new TopLevelProvider(topLevelControl);
        
    }

    public TopLevel GetTopLevel()
    {
        if (!_alreadyFetchedTopLevelControl)
        {
            TryToFetchTopLevel();
            _alreadyFetchedTopLevelControl = true;
        }
        if (_topLevel is null)
            throw new InvalidOperationException(
                "Can not fetch the top level. Please create an instance of this class first.");
        return _topLevel;
    }
    private static void TryToFetchTopLevel()
    {
        if (Instance is null)
            throw new InvalidOperationException(
                "Can not fetch the top level. Please create and instance of the class TopLevelProvider first.");
        if(Instance._topLevel is not null)
            return;
        Instance._topLevel = TopLevel.GetTopLevel(Instance._topLevelControl);
        if (Instance._topLevel is null)
            throw new NullReferenceException("Can not get the TopLevel Control!");
    }
}