using Avalonia.Media;
using BoTech.ApiClient.Base.Editor.Controllers;
using BoTech.ApiClient.Base.Editor.Models.Api;
using BoTech.ApiClient.LibreTranslate;
using ReactiveUI;
using ReactiveUI.Primitives;
using ShadUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BoTech.ApiClient.Base.Editor.Models;
using BoTech.ApiClient.Base.Models.UserMessage;
using BoTech.ApiClient.LibreTranslate.Models.Requests;
using BoTech.ApiClient.LibreTranslate.Models.Responses;


namespace BoTech.ApiClient.Base.Editor.ViewModels.Editor.UserMessages
{
    internal class EditUserMessagesViewModel : ViewModelBase
    {
        /// <summary>
        /// Contains the possible results of the selected endpoint. used to display the results in the listbox
        /// </summary>
        public ObservableCollection<EndpointResultItemViewModel> PossibleResults { get; set; } = new();

        /// <summary>
        /// Contains the selected possible result of the selected endpoint. Selection of the list box on the left side.
        /// </summary>
        public EndpointResultItemViewModel SelectedPossibleResult
        {
            get;
            set
            {
                this.RaiseAndSetIfChanged(ref field, value);
                if (value is not null)
                    OnUpdateUserMessageForHttpResult();
            }
        }
        /// <summary>
        /// Contains the names of all cultures that are available in the system. The names are formatted like this: "en-US (English (United States))"
        /// </summary>
        public ObservableCollection<string> CultureNamesIncludingLanguageName { get; set; } = new();
        /// <summary>
        /// The INDEX of the language that is selected within the language combo box.
        /// </summary>
        public int SelectedSourceCultureIndex { get; set => this.RaiseAndSetIfChanged(ref field, value); } = 1;
        /// <summary>
        /// The current user message that is displayed in the text box. This message will be translated to all supported languages when the user clicks the translate button.
        /// </summary>
        public string CurrentUserMessage { get; set => this.RaiseAndSetIfChanged(ref field, value); }
        /// <summary>
        /// The event for the "translate" button.
        /// </summary>
        public ReactiveCommand<RxVoid, Task> TranslateCurrentUserMessageCommand { get; set; }
        /// <summary>
        /// This command is used to save the current user messages for the selected endpoint.
        /// It will update the <see cref="_userMessagesForSelectedEndpoint"/> object and save it to the selected controller.
        /// </summary>
        public ReactiveCommand<RxVoid, RxVoid> SaveCurrentUserMessageForHttpResultCommand { get; set; }
        /// <summary>
        /// The translated versions of <see cref="CurrentUserMessage"/>
        /// </summary>
        public ObservableCollection<AutoTranslatedUserMessage> AutoTranslatedUserMessages { get; set; } = new();
        /// <summary>
        /// the controller that is selected in the controller nav panel.
        /// </summary>
        private Controller _selectedController;
        /// <summary>
        /// the endpoint that is selected in the controller nav panel.
        /// </summary>
        private Endpoint _selectedEndpoint;
        /// <summary>
        /// The api client for <seealso cref="http://translate.botech.dev"/>
        /// </summary>
        private TranslatorClient _translatorClient;
        /// <summary>
        /// All languages that are supported by the libre translate server.
        /// </summary>
        private List<CultureInfo> _supportedLanguages;
        /// <summary>
        /// The user messages for the selected endpoint. This is used to display the user messages in the listbox.
        /// </summary>
        private UserMessagesForEndpoint _userMessagesForSelectedEndpoint;

        public EditUserMessagesViewModel(DialogManager dialogManager, ToastManager toastManager) : base(dialogManager,
            toastManager)
        {
            EditorViewController.Instance.OnUserSelectedEndpointInControllerNav += OnUpdateUserEndpointSelection;
            TranslateCurrentUserMessageCommand = ReactiveCommand.Create(TranslateCurrentUserMessageToAllLanguages);
            SaveCurrentUserMessageForHttpResultCommand = ReactiveCommand.Create(SaveCurrentUserMessagesForHttpResult);
            InitLanguageCollection();
        }
        /// <summary>
        /// Init's the language combo box, by fetching all supported languages from the libre translate server.
        /// </summary>
        /// <returns></returns>
        private async Task InitLanguageCollection()
        {

            _translatorClient = TranslatorClient.CreateClientUsingSelfHostingServer("https://translate.botech.dev");
            List<SupportedLanguageResult>? supportedLanguages = await _translatorClient.GetSupportedLanguages();
            if (supportedLanguages == null)
                throw new InvalidOperationException("Could not fetch supported languages.");
            _supportedLanguages = new List<CultureInfo>(supportedLanguages.Select(language => new CultureInfo(language.LanguageCode)));
            _supportedLanguages.ForEach(culture =>
            {
                CultureNamesIncludingLanguageName.Add($"{culture.Name} ({culture.DisplayName})");
            });
            SelectedSourceCultureIndex = 0;
        }
        /// <summary>
        /// Updates the selected endpoint and controller when the user selects a new endpoint in the controller nav panel. Also updates the possible results of the selected endpoint.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedEndpointInController"></param>
        private void OnUpdateUserEndpointSelection(object sender, Tuple<Endpoint, Controller> selectedEndpointInController)
        {
            _selectedEndpoint = selectedEndpointInController.Item1;
            _selectedController = selectedEndpointInController.Item2;
            UpdatePossibleResults();
            _userMessagesForSelectedEndpoint = FetchOrCreateUserMessagesForEndpointFromController();
            // reset view:
            AutoTranslatedUserMessages.Clear();
            CurrentUserMessage = "";
        }
        /// <summary>
        /// Fetches the user messages for the selected endpoint from the selected controller.
        /// If no user messages exist for the selected endpoint, a new instance of <see cref="UserMessagesForEndpoint"/> is created.
        /// </summary>
        /// <returns></returns>
        private UserMessagesForEndpoint FetchOrCreateUserMessagesForEndpointFromController()
        {
            UserMessagesForEndpoint userMessages = _selectedController.UserResultMessages.Find(um => um.Endpoint.Equals(_selectedEndpoint.MethodName) && um.HttpPath.Equals(_selectedEndpoint.Route));
            if (userMessages == null)
            {
                userMessages = new UserMessagesForEndpoint(_selectedController.ControllerName, _selectedEndpoint.MethodName, _selectedEndpoint.Route);
            }
            return userMessages;
        }
        /// <summary>
        /// Updates the possible results of the selected endpoint and adds them to <see cref="PossibleResults"/>
        /// This method updates the ui.
        /// </summary>
        private void UpdatePossibleResults()
        {
            PossibleResults.Clear();
            foreach (EndpointResult endpointResult in _selectedEndpoint.PossibleResults)
            {
                PossibleResults.Add(new EndpointResultItemViewModel()
                {
                    ReferencedEndpointResult = endpointResult,
                    ServerResultStatusCode = endpointResult.ServerResultStatusCode.ToString() + " " + (int)endpointResult.ServerResultStatusCode,
                    DtoNameOrReturnTypeName = endpointResult.DtoName == "" ? endpointResult.ResponseType : endpointResult.DtoName,
                    ServerResultIndicatorBackgroundBrush = GetBackgroundColorOfHttpStatusCodeIndicator(endpointResult.ServerResultStatusCode),
                    ServerResultIndicatorBorderBrush = GetBorderColorOfHttpStatusCodeIndicator(endpointResult.ServerResultStatusCode)
                });
            }
        }
        /// <summary>
        /// translates the current user message to all supported languages and adds them to <see cref="AutoTranslatedUserMessages"/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Error by requesting an endpoint at the server</exception>
        private async Task TranslateCurrentUserMessageToAllLanguages()
        {
            AutoTranslatedUserMessages.Clear();
            if (string.IsNullOrEmpty(CurrentUserMessage))
            {
                AutoTranslatedUserMessages.Add(AutoTranslatedUserMessage.Empty);
                return;
            }

            TranslateDto dto = new TranslateDto
            {
                TextToTranslate = CurrentUserMessage,
                SourceLanguageCode = _supportedLanguages[SelectedSourceCultureIndex].TwoLetterISOLanguageName,
                TargetLanguageCode = "en",
                FormatOfTextToTranslate = TextFormat.text,
                CountOfAlternativeTranslations = 3
            };
            TranslationResult? result;
            foreach (CultureInfo supportedLanguage in _supportedLanguages)
            {
                dto.TargetLanguageCode = supportedLanguage.TwoLetterISOLanguageName;
                result = await _translatorClient.Translate(dto);
                if(result == null)
                    throw new InvalidOperationException("Could not translate the message.");
                AutoTranslatedUserMessages.Add(new AutoTranslatedUserMessage()
                {
                    Language = supportedLanguage,
                    TranslatedMessage = result.TranslatedText
                });
            }
        }
        /// <summary>
        /// Saves the current user messages for the selected endpoint result to the <see cref="SelectedPossibleResult"/>. Updates the <see cref="SelectedPossibleResult.ReferencedEndpointResult.Message"/> property.
        /// </summary>
        private void SaveCurrentUserMessagesForHttpResult()
        {
            EndpointResult result = SelectedPossibleResult.ReferencedEndpointResult;
            if (result.Message is null)
                result.Message = new NaturalLanguageMessage();
            result.Message.UserMessages.Clear();
            foreach (AutoTranslatedUserMessage translatedUserMessage in AutoTranslatedUserMessages)
            {
                result.Message.UserMessages.Add(translatedUserMessage.Language, translatedUserMessage.TranslatedMessage);
            }
        }
        /// <summary>
        /// Updates the current user message and the auto translated user messages when the user selects a new possible result in the list box.
        /// Updates the view.
        /// </summary>
        private void OnUpdateUserMessageForHttpResult()
        {
            EndpointResult result = SelectedPossibleResult.ReferencedEndpointResult;
            if (result.Message != null)
            {
                LoadAllAutoTranslatedMessagesFromEndpointResultToView(result);
            }
        }
        /// <summary>
        /// Loads all auto translated messages from the given endpoint result to the view. Updates <see cref="CurrentUserMessage"/> and <see cref="AutoTranslatedUserMessages"/>
        /// </summary>
        /// <param name="result">The endpoint result containing the user messages</param>
        private void LoadAllAutoTranslatedMessagesFromEndpointResultToView(EndpointResult result)
        {
            string selectedLanguageCode = _supportedLanguages[SelectedSourceCultureIndex].TwoLetterISOLanguageName;
            foreach (KeyValuePair<CultureInfo, string> translatedMessage in result.Message.UserMessages)
            {
                if (translatedMessage.Key.TwoLetterISOLanguageName == selectedLanguageCode)
                {
                    CurrentUserMessage = translatedMessage.Value;
                }

                AutoTranslatedUserMessages.Add(new AutoTranslatedUserMessage()
                {
                    Language = translatedMessage.Key,
                    TranslatedMessage = translatedMessage.Value
                });
            }
        }
        /// <summary>
        /// determines the border color of the http status code indicator based on the status code.
        /// </summary>
        /// <param name="status">given http status code</param>
        /// <returns>border brush for the status code indicator</returns>
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
        /// <summary>
        /// determines the background color of the http status code indicator based on the status code.
        /// </summary>
        /// <param name="status">given http status code</param>
        /// <returns>background brush for the status code indicator</returns>
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
