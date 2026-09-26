using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BoTech.ApiClient.Base.Editor.ViewModels.Editor.ControllerNav
{
    internal class ControllerNavItemViewModel
    {
        public string ControllerName { get; set; } = "";

        public ObservableCollection<EndpointItemViewModel> EndpointsDefinedInController { get; set; } = new ObservableCollection<EndpointItemViewModel>();
    }
}
