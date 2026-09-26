using Avalonia.Media;
using BoTech.ApiClient.Base.Editor.Controllers;
using BoTech.ApiClient.Base.Editor.Models.Api;
using ShadUI;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;

namespace BoTech.ApiClient.Base.Editor.ViewModels.Editor.UserMessages
{
    internal class EditUserMessagesViewModel : ViewModelBase
    {
        public ObservableCollection<EndpointResultItemViewModel> PossibleResults { get; set; } = new();

        public EditUserMessagesViewModel(DialogManager dialogManager, ToastManager toastManager) : base(dialogManager,
            toastManager)
        {
            EditorViewController.Instance.OnUserSelectedEndpointInControllerNav += UpdatePossibleResults;
        }

        private void UpdatePossibleResults(object sender, Tuple<Endpoint, Controller> selectedEndpointInController)
        {
            PossibleResults.Clear();
            foreach (EndpointResult endpointResult in selectedEndpointInController.Item1.PossibleResults)
            {
                PossibleResults.Add(new EndpointResultItemViewModel()
                {
                    ServerResultStatusCode = endpointResult.ServerResultStatusCode.ToString() + " " + (int)endpointResult.ServerResultStatusCode,
                    DtoNameOrReturnTypeName = endpointResult.DtoName == "" ? endpointResult.ResponseType : endpointResult.DtoName,
                    ServerResultIndicatorBackgroundBrush = GetBackgroundColorOfHttpStatusCodeIndicator(endpointResult.ServerResultStatusCode),
                    ServerResultIndicatorBorderBrush = GetBorderColorOfHttpStatusCodeIndicator(endpointResult.ServerResultStatusCode)
                });
            }
        }
        private IBrush GetBorderColorOfHttpStatusCodeIndicator(HttpStatusCode status)
        {
            // Informational 1xx
            if ((int)status < 200) 
                return Brushes.Blue;
            // Successful 2xx
            if ((int)status < 300)
                return Brushes.Green;
            // Redirection 3xx
            if ((int)status < 400)
                return Brushes.Orange;
            // Client Error 4xx
            if ((int)status < 500)
                return Brushes.Fuchsia;
            // Server Error 5xx
            if ((int)status < 600)
                return Brushes.Red;
            return Brushes.Gray;
        }
        private IBrush GetBackgroundColorOfHttpStatusCodeIndicator(HttpStatusCode status)
        {
            // Informational 1xx
            if ((int)status < 200)
                return Brushes.LightBlue;
            // Successful 2xx
            if ((int)status < 300)
                return Brushes.LightGreen;
            // Redirection 3xx
            if ((int)status < 400)
                return Brush.Parse("#FBF1E6");
            // Client Error 4xx
            if ((int)status < 500)
                return Brushes.LightPink;
            // Server Error 5xx
            if ((int)status < 600)
                return Brushes.LightSalmon;
            return Brushes.LightGray;
        }
    }
}
