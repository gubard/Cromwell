using System.Collections;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell.Helpers;

namespace Cromwell.Models;

public abstract class ViewModelBase : ObservableObject, INotifyDataErrorInfo
{
    private bool _isAnyExecute;
    private readonly Dictionary<string, Func<IEnumerable<ValidationError>>> _errors = new();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors => _errors.Count != 0 && _errors.Any(x => x.Value.Invoke().Any());

    protected Task WrapCommand(Func<Task> func)
    {
        StartExecute();

        if (HasErrors)
        {
            return Task.CompletedTask;
        }

        return UiHelper.ExecuteAsync(func);
    }

    protected void StartExecute()
    {
        _isAnyExecute = true;

        foreach (var error in _errors)
        {
            ErrorsChanged?.Invoke(this, new(error.Key));
        }
    }

    public IEnumerable GetErrors(string? propertyName)
    {
        if (_isAnyExecute is false)
        {
            return Enumerable.Empty<ValidationError>();
        }

        if (propertyName is null)
        {
            return Enumerable.Empty<ValidationError>();
        }

        return _errors.TryGetValue(propertyName, out var validation) ? validation.Invoke() : [];
    }

    protected void SetValidation(string propertyName, Func<IEnumerable<ValidationError>> validation)
    {
        _errors[propertyName] = validation;
    }
}