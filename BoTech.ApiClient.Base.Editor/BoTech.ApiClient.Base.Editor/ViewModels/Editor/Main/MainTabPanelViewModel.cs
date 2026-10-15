using ReactiveUI;
using ReactiveUI.Primitives;
using ShadUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Avalonia.Controls;
using BoTech.ApiClient.Base.Editor.ViewModels.Editor.UserMessages;
using BoTech.ApiClient.Base.Editor.Views.Editor.UserMessages;

namespace BoTech.ApiClient.Base.Editor.ViewModels.Editor.Main
{
    internal class MainTabPanelViewModel : ViewModelBase
    {
        public ObservableCollection<TabPage> Tabs { get; set; } = new ObservableCollection<TabPage>();

        public ReactiveCommand<TabPage, RxVoid> CloseSpecificTabCommand { get; set; }

        private static MainTabPanelViewModel? _currentInstance;

        public MainTabPanelViewModel() : base(null, null)
        {
            Tabs.Add(new TabPage("User Messages", new TextBlock()
            {
                Text = "Bello content"
            }));
        }
        public MainTabPanelViewModel(DialogManager dialogManager, ToastManager toastManager) : base(dialogManager, toastManager)
        {
            CloseSpecificTabCommand = ReactiveCommand.Create<TabPage>((tabToClose) =>
            {
                Tabs.Remove(tabToClose);
            });
            _currentInstance = this;
        }
        /// <summary>
        /// displays all views that are assigned to this tab control.
        /// </summary>
        public static void ShowAllViews()
        {
            ShowEditUserMessagesView();
        }
        /// <summary>
        /// Adds the <see cref="EditUserMessagesView"/> view as a tab to the main window.
        /// </summary>
        /// <exception cref="InvalidOperationException">When <see cref="_currentInstance"/> is null.</exception>
        public static void ShowEditUserMessagesView()
        {
            if (_currentInstance is null)
                throw new InvalidOperationException(
                    "Please create an instance of the MainTabPanelViewModel class first, before adding tabs.");
            _currentInstance.Tabs.Add(new TabPage("User Messages", new EditUserMessagesView()
            {
                DataContext = new EditUserMessagesViewModel(_currentInstance.DialogManager, _currentInstance.ToastManager)
            }));
        }

        internal class TabPage(string header, Control content)
        {
            public string Header { get; set; } = header;
            public Control Content { get; set; } = content;
        }
    }
}
