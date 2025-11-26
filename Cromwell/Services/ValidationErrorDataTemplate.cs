using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Gaia.Errors;
using Inanna.Helpers;
using Inanna.Services;

namespace Cromwell.Services;

public class ValidationErrorDataTemplate : IDataTemplate
{
    private readonly IApplicationResourceService _applicationResourceService =
        DiHelper.ServiceProvider.GetService<IApplicationResourceService>();

    private readonly IStringFormater _stringFormater = DiHelper.ServiceProvider.GetService<IStringFormater>();

    public Control? Build(object? param)
    {
        return param switch
        {
            null => null,
            PropertyZeroValidationError zero => new()
            {
                Text = _stringFormater.Format(
                    _applicationResourceService.GetResource<string>("Lang.PropertyZeroValidationError"),
                    _applicationResourceService.GetResource<string>($"Lang.{zero.PropertyName}")),
            },
            PropertyEmptyValidationError empty => new()
            {
                Text = _stringFormater.Format(
                    _applicationResourceService.GetResource<string>("Lang.PropertyEmptyValidationError"),
                    _applicationResourceService.GetResource<string>($"Lang.{empty.PropertyName}")),
            },
            _ => new TextBlock
            {
                Text = $"Not found \"{param.GetType()}\"",
            },
        };
    }

    public bool Match(object? data)
    {
        return data is ValidationError;
    }
}