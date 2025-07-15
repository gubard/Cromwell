using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Db;
using Cromwell.Generator;
using Cromwell.Models;

namespace Cromwell.ViewModels;

[EditNotify]
public partial class EditCredentialViewModel : ViewModelBase
{
    public EditCredentialViewModel(Guid id)
    {
        Id = id;

        SetValidation(nameof(Name),
            () => string.IsNullOrWhiteSpace(Name) ? [new PropertyEmptyValidationError(nameof(Name)),] : []);

        SetValidation(nameof(Login),
            () => string.IsNullOrWhiteSpace(Login) ? [new PropertyEmptyValidationError(nameof(Login)),] : []);

        SetValidation(nameof(Key),
            () => string.IsNullOrWhiteSpace(Key) ? [new PropertyEmptyValidationError(nameof(Key)),] : []);

        SetValidation(nameof(Length), () => Length == 0 ? [new PropertyZeroValidationError(nameof(Length)),] : []);

        SaveCommand = new AsyncRelayCommand(() =>
        {
            StartExecute();

            if (HasErrors)
            {
                return Task.CompletedTask;
            }
            
            return Task.CompletedTask;
        });
    }

    public Guid Id { get; }
    public ICommand SaveCommand { get; }

    [ObservableProperty]
    public partial bool IsEditName { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsEditLogin { get; set; }

    [ObservableProperty]
    public partial string Login { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsEditKey { get; set; }

    [ObservableProperty]
    public partial string Key { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsEditIsAvailableUpperLatin { get; set; }

    [ObservableProperty]
    public partial bool IsAvailableUpperLatin { get; set; }

    [ObservableProperty]
    public partial bool IsEditIsAvailableLowerLatin { get; set; }

    [ObservableProperty]
    public partial bool IsAvailableLowerLatin { get; set; }

    [ObservableProperty]
    public partial bool IsEditIsAvailableNumber { get; set; }

    [ObservableProperty]
    public partial bool IsAvailableNumber { get; set; }

    [ObservableProperty]
    public partial bool IsEditIsAvailableSpecialSymbols { get; set; }

    [ObservableProperty]
    public partial bool IsAvailableSpecialSymbols { get; set; }

    [ObservableProperty]
    public partial bool IsEditCustomAvailableCharacters { get; set; }

    [ObservableProperty]
    public partial string CustomAvailableCharacters { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsEditLength { get; set; }

    [ObservableProperty]
    public partial ushort Length { get; set; }

    [ObservableProperty]
    public partial bool IsEditRegex { get; set; }

    [ObservableProperty]
    public partial string Regex { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsEditType { get; set; }

    [ObservableProperty]
    public partial CredentialType Type { get; set; }
}