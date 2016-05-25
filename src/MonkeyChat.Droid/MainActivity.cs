using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using ImageCircle.Forms.Plugin.Droid;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;


namespace MonkeyChat.Droid
{
    [Activity(Label = "Monkey Chat", Icon = "@mipmap/ic_launcher", Theme= "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            ToolbarResource = Resource.Layout.toolbar;
            TabLayoutResource = Resource.Layout.tabs;

            base.OnCreate(bundle);
            Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            ImageCircleRenderer.Init();

            DependencyService.Register<ITwilioMessenger, TwilioMessenger>();

            LoadApplication(new App());
        }

    }
}

