using Avalonia.Controls.ApplicationLifetimes;
using BoTech.HttpClientHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;

namespace BoTech.ApiClient.Base.Editor.Services
{
    internal class ApplicationUpdateService
    {

        /// <summary>
        /// Contains info about the installed version.
        /// </summary>
        public UpdateInfo CurrentVersion { get; private set; }
        /// <summary>
        /// This event will be invoked when this class finds a new update.
        /// </summary>
        public EventHandler<UpdateInfo> OnNewUpdateFound;
        /// <summary>
        /// This event will be called when there is an error by fetching the update infos from the server or when there is an error by executing the update.
        /// </summary>
        public EventHandler<Exception> OnUpdateError;
        private UpdateInfo? _nextVersion = null;

        private static ApplicationUpdateService? Instance = null;

        public static ApplicationUpdateService GetInstance()
        {
            if(Instance is null)
                Instance = new ApplicationUpdateService();
            return Instance;
        }

        private ApplicationUpdateService()
        {
            CurrentVersion = new UpdateInfo()
            {
                IsLatest = false,
                VersionString = "v1.0.5.Alpha",
                ReleaseDateTime = DateTime.Parse("22:00:00 23.09.2026"),
                InformationString = "This is the current Version you installed. For Release details see the github page: https://github.com/BoTech-Development/BoTech.ApiClient.Base"
            };
        }

        public async Task CheckForUpdates()
        {
            HttpRequestHelper requestHelper = new HttpRequestHelper("https://assets.botech.dev/");
            RequestResult<string> getAllVersionsResult = await requestHelper.HttpGetFileContents("static-app-update/BoTech.ApiClient.Base.Editor/versions.json");
            //RequestResult<List<UpdateInfo>> getAllVersionsResult = requestHelper.HttpGetJsonObject<List<UpdateInfo>>("/static-app-update/BoTech.ApiClient.Base.Editor/versions.json").Result;
            if (getAllVersionsResult.IsSuccess() && getAllVersionsResult.ParsedData != null)
            {
                List<UpdateInfo>? versions = JsonConvert.DeserializeObject<List<UpdateInfo>>(getAllVersionsResult.ParsedData);
                if (versions == null) 
                    throw new InvalidOperationException("Could not parse the versions.json file.");
                _nextVersion = FindLatestVersionInGetAllVersionsResult(versions);
                if(IsUpdateAvailable())
                    OnNewUpdateFound.Invoke(this, _nextVersion!); // ! because we checked in the if-statement.
            }
            else if(getAllVersionsResult.Error != null)
            {
                OnUpdateError.Invoke(this, getAllVersionsResult.Error);
            }
        }

        private UpdateInfo? FindLatestVersionInGetAllVersionsResult(List<UpdateInfo> versions)
        {
            UpdateInfo? latestVersion = versions.Find(ui => ui.IsLatest);// null of parse data checked by CheckForUpdates().
            if (latestVersion != null && !latestVersion.VersionString.Equals(CurrentVersion.VersionString) && !latestVersion.ReleaseDateTime.Equals(CurrentVersion.ReleaseDateTime))
            {
                return latestVersion;
            }
            return null;
        }

        public bool IsUpdateAvailable()
        {
            return _nextVersion != null;
        }

        public UpdateInfo GetNewVersionInfo()
        {
            if( _nextVersion == null)
                throw new InvalidOperationException("There is no update available.");
            return _nextVersion;
        }

        public void ExecuteUpdateCommand()
        {
            if (!IsUpdateAvailable())
                throw new InvalidOperationException("There is no update available. Thus can not execute the update.");
            DirectoryInfo? parentDirOfCurrentDirectory = Directory.GetParent(Directory.GetCurrentDirectory());
            if (parentDirOfCurrentDirectory is null)
                throw new InvalidOperationException("The application cannot be installed in the root directory of your drive!");
            string scriptFilePath = Path.Combine(parentDirOfCurrentDirectory.FullName, "update.ps1");
            if(!ExecuteACommandWithWaitForExit($"Invoke-WebRequest -Uri 'https://assets.botech.dev/static-app-update/BoTech.ApiClient.Base.Editor/update.ps1' -OutFile '{scriptFilePath}'", Directory.GetCurrentDirectory()))
                return; // if one command fails, stop the execution.
            if (!ExecuteACommand(scriptFilePath, parentDirOfCurrentDirectory.FullName))
                return;
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopApp)
            {
                desktopApp.Shutdown();
            }
            else
            {
                Console.WriteLine("Please close app manually.");
            }
        }
        /// <summary>
        /// Executes the command in a new window.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>true when success, else false</returns>
        private bool ExecuteACommandWithWaitForExit(string command, string workingDirectory)
        {
            try
            {
                ProcessStartInfo psi = BuildPowershellCommandExecutionProgress(command, workingDirectory);
                Process.Start(psi)!.WaitForExit();
                return true;
            }
            catch (Exception ex)
            {
                OnUpdateError?.Invoke(this, ex);
                return false;
            }
        }

        private bool ExecuteACommand(string command, string workingDirectory)
        {
            try
            {
                ProcessStartInfo psi = BuildPowershellCommandExecutionProgress(command, workingDirectory);
                Process.Start(psi);
                return true;
            }
            catch (Exception ex)
            {
                OnUpdateError?.Invoke(this, ex);
                return false;
            }
        }

        private ProcessStartInfo BuildPowershellCommandExecutionProgress(string command, string workingDirectory)
        {
            return new ProcessStartInfo
            {
                FileName = "pwsh.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                UseShellExecute = true,
                CreateNoWindow = false,
                WorkingDirectory = workingDirectory,
                WindowStyle = ProcessWindowStyle.Normal
            };
        }

        public class UpdateInfo
        {
            /// <summary>
            /// only true if the version is the latest.
            /// </summary>
            public bool IsLatest { get; init; }
            /// <summary>
            /// The version string defined at the following page: https://docs.botech.dev/botech-versionpolicy/v1-1/about/
            /// </summary>
            public string VersionString { get; init; }
            /// <summary>
            /// Infos for the user. For instance release details.
            /// </summary>
            public string InformationString { get; init; }
            /// <summary>
            /// The release date of the version.
            /// </summary>
            public DateTime ReleaseDateTime { get; init; }
        }
    }
}
