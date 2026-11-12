using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Net.Http;
using Avalonia.Media;
using BoTech.ApiClient.Base.Editor.Controllers;
using BoTech.ApiClient.Base.Editor.Models;
using BoTech.ApiClient.Base.Editor.Models.Api;

namespace BoTech.ApiClient.Base.Editor.ViewModels.Editor.ControllerNav
{
    internal class ControllerNavPanelViewModel : ViewModelBase
    {
        public ObservableCollection<ControllerNavItemViewModel> UserDefinedControllers
        {
            get => field;
            set => this.RaiseAndSetIfChanged(ref field, value);
        } = new ObservableCollection<ControllerNavItemViewModel>();
        /// <summary>
        /// Only to invoke the Events which notify all views to load the data for the select controller or endpoint.
        /// </summary>
        public object? SelectedEndpointOrController
        {
            get;
            set
            {
                InvokeChangeSelectionEventFor(value);
                field = value;
            }
        }
        /// <summary>
        /// Invokes the OnUserSelectedControllerInControllerNav in <see cref="EditorViewController"/> or OnUserSelectedEndpointInControllerNav depending on the selected View Model
        /// </summary>
        /// <param name="selectedObject">The selected View Model</param>
        private void InvokeChangeSelectionEventFor(object? selectedObject)
        {
            if (selectedObject != null)
                if (selectedObject is ControllerNavItemViewModel controllerVm)
                {
                    EditorViewController.Instance.OnUserSelectedControllerInControllerNav?.Invoke(this, ConvertViewModelBack(controllerVm));
                }
                else if (selectedObject is EndpointItemViewModel endpointVm)
                {
                    EditorViewController.Instance.OnUserSelectedEndpointInControllerNav?.Invoke(this, ConvertViewModelBack(endpointVm));
                }
        }
        /// <summary>
        /// Converts the <see cref="ControllerNavItemViewModel"/> to the corresponding Model <see cref="Controller"/>>.
        /// </summary>
        /// <param name="controllerViewModel">the vm to convert.</param>
        /// <returns>The actual representation</returns>
        /// <exception cref="InvalidOperationException">When not found.</exception>
        private Controller ConvertViewModelBack(ControllerNavItemViewModel controllerViewModel)
        {
            Controller? result = ProjectController.GetInstance().CurrentLoadedProject.CustomizedControllers.Find((controller) =>
                controller.ControllerName.Equals(controllerViewModel.ControllerName));
            if (result is null)
                throw new InvalidOperationException("Can not find the selected Endpoint!");
            return result;
        }
        /// <summary>
        /// Converts the <see cref="ControllerNavItemViewModel"/> to the corresponding Model <see cref="Endpoint"/>>.
        /// </summary>
        /// <param name="endpointVm">the vm to convert.</param>
        /// <returns>The actual representation</returns>
        /// <exception cref="InvalidOperationException">When not found.</exception>
        private Tuple<Endpoint,Controller> ConvertViewModelBack(EndpointItemViewModel endpointVm)
        {
            foreach (Controller controller in ProjectController.GetInstance().CurrentLoadedProject.CustomizedControllers)
            {
                Endpoint? result =
                    controller.DefinedEndpointsInController.Find(endpointVm.Equals);
                if (result is not null)
                    return new Tuple<Endpoint, Controller>(result, controller);
            }
            throw new InvalidOperationException("Can not find the selected Endpoint!");
        }
        public ControllerNavPanelViewModel()
        {
            InitView(ProjectController.GetInstance().CurrentLoadedProject);
        }
        /// <summary>
        /// 1. converts all controllers / endpoints into the following type: <see cref="ControllerNavItemViewModel"/>
        /// 2. Displays all converted models in the list box.
        /// </summary>
        /// <param name="loadedProject">Current project which contains the controller and endpoints.</param>
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
                        EndpointName = endpoint.MethodName,
                        HttpMethodIndicatorBackgroundColor = GetBackgroundColorOfHttpMethodIndicator(endpoint.HttpMethod),
                        HttpMethodIndicatorBorderColor = GetBorderColorOfHttpMethodIndicator(endpoint.HttpMethod),
                        HttpMethodName = endpoint.HttpMethod.Method,
                        Route = endpoint.Route
                    });
                }
                UserDefinedControllers.Add(controllerVm);
            }
        }

        private IBrush GetBorderColorOfHttpMethodIndicator(HttpMethod method)
        {
            switch (method.Method)
            {
                case "GET":
                    return Brush.Parse("#FFA4E0A7"); // Green
                case "POST":
                    return Brush.Parse("#FF98CDFD");
                case "PUT":
                    return Brush.Parse("#FFFED998");
                case "PATCH":
                    return Brush.Parse("#FFDD9BE0");
                case "DELETE":
                    return Brush.Parse("#FFFDB7A5");
                default:
                    return Brush.Parse("#FFC6CACD");
            }
        }
        private IBrush GetBackgroundColorOfHttpMethodIndicator(HttpMethod method)
        {
            switch (method.Method)
            {
                case "GET":
                    return Brush.Parse("#FFD0F0D1");
                case "POST":
                    return Brush.Parse("#FFCBE7FE");
                case "PUT":
                    return Brush.Parse("#FFFEEECC");
                case "PATCH":
                    return Brush.Parse("#FFEFCAF0");
                case "DELETE":
                    return Brush.Parse("#FFFEDDD2");
                default:
                    return Brush.Parse("#FFE6E8EA");
            }
        }
    }
}
