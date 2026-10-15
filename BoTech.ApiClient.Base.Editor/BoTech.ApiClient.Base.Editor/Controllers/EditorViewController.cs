using System;
using System.Collections.Generic;
using System.Text;
using BoTech.ApiClient.Base.Editor.Models;
using BoTech.ApiClient.Base.Editor.Models.Api;
using BoTech.ApiClient.Base.Editor.ViewModels;
using BoTech.ApiClient.Base.Editor.ViewModels.Editor;
using BoTech.ApiClient.Base.Editor.ViewModels.Editor.ControllerNav;
using BoTech.ApiClient.Base.Editor.ViewModels.Editor.Main;
using BoTech.ApiClient.Base.Editor.Views.Editor;
using BoTech.ApiClient.Base.Editor.Views.Editor.Main;

namespace BoTech.ApiClient.Base.Editor.Controllers
{
    /// <summary>
    /// This class is a singleton, which controls the ui of the editor.
    /// </summary>
    internal class EditorViewController
    {
        /// <summary>
        /// Invoked by the <see cref="ControllerNavPanelViewModel"/> when the user selects a specific Controller in the list box
        /// </summary>
        public EventHandler<Models.Api.Controller> OnUserSelectedControllerInControllerNav;
        /// <summary>
        /// Invoked by the <see cref="ControllerNavPanelViewModel"/> when the user selects a specific Endpoint in the list box.
        /// This event also provides the controller where the endpoint is defined.
        /// </summary>
        public EventHandler<Tuple<Endpoint, Controller>> OnUserSelectedEndpointInControllerNav;
        /// <summary>
        /// Necessary to the control the view from this class.
        /// </summary>
        private MainViewModel _mainViewModel;
        /// <summary>
        /// singleton
        /// </summary>
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

        public void InitializeViewsForCurrentProject()
        {
            _mainViewModel.ControllerNavPanelView = new ControllerNavPanelView()
            {
                DataContext = new ControllerNavPanelViewModel(_mainViewModel.DialogManager,
                    _mainViewModel.ToastManager)
            };
            _mainViewModel.MainTabPanelView = new MainTabPanelView()
            {
                DataContext = new MainTabPanelViewModel(_mainViewModel.DialogManager, _mainViewModel.ToastManager)
            };
            MainTabPanelViewModel.ShowAllViews();
        }
    }
}
