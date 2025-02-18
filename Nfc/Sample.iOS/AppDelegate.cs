﻿using Foundation;
using Shiny;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


namespace Sample.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            this.ShinyFinishedLaunching(new Startup(), options);
            Forms.Init();
            this.LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }
    }
}
