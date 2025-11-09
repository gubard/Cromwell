using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Cromwell.Ui;
using Inanna.Helpers;
using Inanna.Services;
using Microsoft.EntityFrameworkCore;

namespace Cromwell;

public class App : InannaApplication
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
        var viewModel = DiHelper.ServiceProvider.GetService<MainViewModel>();
        var dbContext = DiHelper.ServiceProvider.GetService<DbContext>();
        dbContext.Database.EnsureCreated();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel,
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = viewModel,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}