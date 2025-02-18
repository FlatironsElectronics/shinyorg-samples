﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Navigation;
using Shiny;
using Shiny.Extensions.Localization;
using Xamarin.Forms;


namespace Sample
{
    public class Startup : FrameworkStartup
    {
        // all of your prism & shiny registrations in one place
        protected override void Configure(ILoggingBuilder builder, IServiceCollection services)
        {
            // handy dialogs
            services.UseXfMaterialDialogs();

            // command exception handling - writes to your error log & displays to a dialog, depending on how you configure below
            services.UseGlobalCommandExceptionHandler(cfg =>
            {
                cfg.LogError = true;
                cfg.IgnoreTokenCancellations = true; // these are generally page cancellations or http timeouts - you generally want to ignore task cancellation exceptions
#if DEBUG
                cfg.AlertType = ErrorAlertType.FullError; // show us the entire error in our alert
#else
                // this will pull out of you ILocalize which is registered with UseResxLocatization below
                cfg.LocalizeErrorBodyKey = "ErrorBody";
                cfg.LocalizeErrorTitleKey = "ErrorTitle";
                cfg.AlertType = ErrorAlertType.Localize;
#endif
            });

            // localization done right
            var localization = new LocalizationBuilder()
                .AddResource("Sample.Resources.Strings", this.GetType().Assembly, "Strings")
                .Build();

            services.AddSingleton(localization);
        }


        public override void ConfigureApp(Application app, IContainerRegistry containerRegistry)
        {
            // register your viewmodels and any services that are specific only to your UI
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainViewModel>();
        }


        public override Task RunApp(INavigationService navigator)
        {
            // perform your inital navigation here
            return navigator.Navigate("/NavigationPage/MainPage");
        }
    }
}
