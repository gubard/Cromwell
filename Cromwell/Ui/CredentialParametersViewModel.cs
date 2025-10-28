using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell.Db;
using Cromwell.Generator;
using Gaia.Extensions;
using Inanna.Errors;
using Inanna.Models;

namespace Cromwell.Ui;

[EditNotify]
public partial class CredentialParametersViewModel : ViewModelBase
{
    public CredentialParametersViewModel(CredentialEntity entity) : this(entity.Id)
    {
        Name = entity.Name;
        Login = entity.Login;
        Key = entity.Key;
        IsAvailableUpperLatin = entity.IsAvailableUpperLatin;
        IsAvailableLowerLatin = entity.IsAvailableLowerLatin;
        IsAvailableNumber = entity.IsAvailableNumber;
        IsAvailableSpecialSymbols = entity.IsAvailableSpecialSymbols;
        CustomAvailableCharacters = entity.CustomAvailableCharacters;
        Length = entity.Length;
        Regex = entity.Regex;
        Type = entity.Type;
    }

    public CredentialParametersViewModel(Guid id)
    {
        Id = id;

        SetValidation(nameof(Name),
            () => string.IsNullOrWhiteSpace(Name) ? [new PropertyEmptyValidationError(nameof(Name)),] : []);

        SetValidation(nameof(Login),
            () => string.IsNullOrWhiteSpace(Login) ? [new PropertyEmptyValidationError(nameof(Login)),] : []);

        SetValidation(nameof(Key),
            () => string.IsNullOrWhiteSpace(Key) ? [new PropertyEmptyValidationError(nameof(Key)),] : []);

        SetValidation(nameof(Length), () => Length == 0 ? [new PropertyZeroValidationError(nameof(Length)),] : []);

        SetValidation(nameof(IsAvailableLowerLatin),
            () => !IsAvailableLowerLatin
             && !IsAvailableNumber
             && !IsAvailableSpecialSymbols
             && !IsAvailableUpperLatin
             && CustomAvailableCharacters.IsNullOrWhiteSpace()
                    ? [new PropertyEmptyValidationError("AvailableCharacters"),]
                    : []);

        Length = 512;
        Children = new();
        Name = string.Empty;
        Login = string.Empty;
        Key = string.Empty;
        CustomAvailableCharacters = string.Empty;
        Regex = string.Empty;
    }

    public Guid Id { get; }
    public AvaloniaList<CredentialParametersViewModel> Children { get; }

    [ObservableProperty]
    public partial bool IsEditName { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial bool IsEditLogin { get; set; }

    [ObservableProperty]
    public partial string Login { get; set; }

    [ObservableProperty]
    public partial bool IsEditKey { get; set; }

    [ObservableProperty]
    public partial string Key { get; set; }

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
    public partial string CustomAvailableCharacters { get; set; }

    [ObservableProperty]
    public partial bool IsEditLength { get; set; }

    [ObservableProperty]
    public partial ushort Length { get; set; }

    [ObservableProperty]
    public partial bool IsEditRegex { get; set; }

    [ObservableProperty]
    public partial string Regex { get; set; }

    [ObservableProperty]
    public partial bool IsEditType { get; set; }

    [ObservableProperty]
    public partial CredentialType Type { get; set; }
}