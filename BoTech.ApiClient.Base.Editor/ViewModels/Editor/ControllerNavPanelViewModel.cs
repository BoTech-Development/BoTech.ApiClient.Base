using BoTech.ApiClient.Base.Editor.ViewModels.Editor.ControllerNavigation;
using ReactiveUI;
using ShadUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BoTech.ApiClient.Base.Editor.Models;
using BoTech.ApiClient.Base.Editor.Models.Api;

namespace BoTech.ApiClient.Base.Editor.ViewModels.Editor
{
    internal class ControllerNavPanelViewModel : ViewModelBase
    {
        public ObservableCollection<ControllerNavItemViewModel> UserDefinedControllers
        {
            get => field;
            set => this.RaiseAndSetIfChanged(ref field, value);
        } = new ObservableCollection<ControllerNavItemViewModel>();

        public ControllerNavPanelViewModel(DialogManager dialogManager, ToastManager toastManager, Project loadedProject) : base(dialogManager, toastManager)
        {
            InitView(loadedProject);
        }

        public void InitView(Project loadedProject)
        {
            foreach (Models.Api.Controller controller in loadedProject.CustomizedControllers)
            {
                ControllerNavItemViewModel controllerVm = new ControllerNavItemViewModel()
                {
                    ControllerName = controller.ControllerName
                };
                foreach (Endpoint endpoint in controller.DefinedEndpointsInController)
                {
                    controllerVm.EndpointsDefinedInController.Add(new EndpointItemViewModel()
                    {
                        EndpointName = endpoint.MethodName
                    });
                }
                UserDefinedControllers.Add(controllerVm);
            }
        }
        
    }
}
