using BoTech.ApiClient.Base.Editor.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ShadUI;
using System;
using System.Collections.Generic;
using System.Text;
using BoTech.ApiClient.Base.Editor.ViewModels.Dialogs;
using BoTech.ApiClient.Base.Editor.Views.Dialogs;

namespace BoTech.ApiClient.Base.Editor.Services
{
    /// <summary>
    /// This class manages all singletons as Services and injects it through dependency injection into the classes that needs an instance of the object.
    /// </summary>
    public class ServiceGenerator
    {
        public static ServiceCollection? Services { get; private set; }
        public static ServiceProvider? ServiceProvider { get; private set; }
        /// <summary>
        /// Creates the <see cref="ServiceProvider"/> instance and the <see cref="Services"/> List.
        /// </summary>
        public static void CreateServices()
        {
            Services = new ServiceCollection();
            Services.AddKeyedSingleton(typeof(DialogManager), null);
            Services.AddKeyedSingleton(typeof(ToastManager), null);
            Services.AddTransient(typeof(MainViewModel));
            ServiceProvider = Services.BuildServiceProvider();
        }

        public static void InitializeDialogManager()
        {
            if (ServiceProvider == null) throw new InvalidOperationException("ServiceProvider not initialized");
            if (ServiceProvider.GetService(typeof(DialogManager)) is DialogManager manager)
            {
                manager.Register<AboutDialogView, AboutDialogViewModel>();
               // manager.Register<ManageProjectDependenciesView, ManageProjectDependenciesViewModel>();
            }
        }
    }
}
