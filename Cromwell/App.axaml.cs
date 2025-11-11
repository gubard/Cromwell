using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
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
        DisableAvaloniaDataAnnotationValidation();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
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