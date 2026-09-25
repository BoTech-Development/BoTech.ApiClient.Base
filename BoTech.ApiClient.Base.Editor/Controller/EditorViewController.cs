using System;
using System.Collections.Generic;
using System.Text;
using BoTech.ApiClient.Base.Editor.Models;
using BoTech.ApiClient.Base.Editor.ViewModels;
using BoTech.ApiClient.Base.Editor.ViewModels.Editor;
using BoTech.ApiClient.Base.Editor.ViewModels.Editor.ControllerNavigation;
using BoTech.ApiClient.Base.Editor.Views.Editor;

namespace BoTech.ApiClient.Base.Editor.Controller
{
    /// <summary>
    /// This class is a singleton, which controls the ui of the editor.
    /// </summary>
    internal class EditorViewController
    {
        private MainViewModel _mainViewModel;
        public static EditorViewController? Instance { get; set; } = null;

        private EditorViewController(MainViewModel vm)
        {
            _mainViewModel = vm;
        }

        public static EditorViewController CreateInstance(MainViewModel vm)
        {
            if(Instance is null)
                Instance = new EditorViewController(vm);
            return Instance;
        }

        public void InitializeViewsForProject(Project loadedProject)
        {
            _mainViewModel.ControllerNavPanelView = new ControllerNavPanelView()
            {
                DataContext = new ControllerNavPanelViewModel(_mainViewModel.DialogManagerProperty,
                    _mainViewModel.ToastManager, loadedProject)
            };
        }
    }
}
