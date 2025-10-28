using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Cromwell.Services;
using Inanna.Helpers;

namespace Cromwell.Android;

[Activity(
    Label = "Cromwell.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        DiHelper.ServiceProvider = new ServiceProvider();
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}
