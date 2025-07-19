using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Models;
using Cromwell.Services;
using Cromwell.Ui;

namespace Cromwell.Helpers;

public static class UiHelper
{
    private static readonly IDialogService DialogService;
    private static readonly IApplicationResourceService ApplicationResourceService;

    static UiHelper()
    {
        DialogService = DiHelper.ServiceProvider.GetService<IDialogService>();
        ApplicationResourceService = DiHelper.ServiceProvider.GetService<IApplicationResourceService>();
        EmptyCommand = new RelayCommand(() => { });

        CancelButton = new(ApplicationResourceService.GetResource<string>("Lang.Cancel"),
            new RelayCommand(() => DialogService.CloseMessageBox()), DialogButtonType.Normal);

        OkButton = new(ApplicationResourceService.GetResource<string>("Lang.Ok"),
            new RelayCommand(() => DialogService.CloseMessageBox()), DialogButtonType.Primary);

        var application = DiHelper.ServiceProvider.GetService<Application>();

        if (Design.IsDesignMode)
        {
            TopLevel = new Window();
        }
        else
        {
            if (application.ApplicationLifetime is null)
            {
                throw new NullReferenceException(nameof(application.ApplicationLifetime));
            }

            TopLevel = application.ApplicationLifetime switch
            {
                IClassicDesktopStyleApplicationLifetime desktop => TopLevel.GetTopLevel(desktop.MainWindow)
                 ?? throw new NullReferenceException(nameof(desktop.MainWindow)),
                ISingleViewApplicationLifetime singleView => TopLevel.GetTopLevel(singleView.MainView)
                 ?? throw new NullReferenceException(nameof(singleView.MainView)),
                _ => throw new NotSupportedException(application.ApplicationLifetime.GetType().FullName),
            };
        }
    }

    public static readonly DialogButton CancelButton;
    public static readonly DialogButton OkButton;
    public static readonly ICommand EmptyCommand;
    public static readonly TopLevel TopLevel;

    public static async Task ExecuteAsync(Func<Task> func)
    {
        try
        {
            await func.Invoke();
        }
        catch (Exception e)
        {
            await DialogService.ShowMessageBoxAsync(new(ApplicationResourceService.GetResource<string>("Lang.Error"),
                new ExceptionViewModel(e), OkButton));
        }
    }
}