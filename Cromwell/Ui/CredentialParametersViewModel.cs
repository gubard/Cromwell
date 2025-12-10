using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell.Models;
using Gaia.Helpers;
using Gaia.Models;
using Inanna.Generator;
using Inanna.Models;
using Turtle.Contract.Models;

namespace Cromwell.Ui;

[EditNotify]
public partial class CredentialParametersViewModel : ViewModelBase
{
    public CredentialParametersViewModel(CredentialNotify item, ValidationMode validationMode, bool isShowEdit)
    {
        IsShowEdit = isShowEdit;
        InitValidation(validationMode);
        Name = item.Name;
        Login = item.Login;
        Key = item.Key;
        IsAvailableUpperLatin = item.IsAvailableUpperLatin;
        IsAvailableLowerLatin = item.IsAvailableLowerLatin;
        IsAvailableNumber = item.IsAvailableNumber;
        IsAvailableSpecialSymbols = item.IsAvailableSpecialSymbols;
        CustomAvailableCharacters = item.CustomAvailableCharacters;
        Length = item.Length;
        Regex = item.Regex;
        Type = item.Type;
        ResetEdit();
    }

    public CredentialParametersViewModel(ValidationMode validationMode, bool isShowEdit)
    {
        IsShowEdit = isShowEdit;
        InitValidation(validationMode);
        Length = 512;
        Name = string.Empty;
        Login = string.Empty;
        Key = string.Empty;
        CustomAvailableCharacters = string.Empty;
        Regex = string.Empty;
        IsAvailableNumber = true;
        IsAvailableLowerLatin = true;
        IsAvailableUpperLatin = true;
        ResetEdit();
    }
    
    public bool IsShowEdit { get; }
    
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

    public EditCredential CreateEditCredential()
    {
        return new()
        {
            CustomAvailableCharacters = CustomAvailableCharacters,
            IsAvailableLowerLatin =
                IsAvailableLowerLatin,
            IsAvailableNumber =
                IsAvailableNumber,
            IsAvailableSpecialSymbols = IsAvailableSpecialSymbols,
            IsAvailableUpperLatin =
                IsAvailableUpperLatin,
            IsEditCustomAvailableCharacters = IsEditCustomAvailableCharacters,
            IsEditIsAvailableLowerLatin = IsEditIsAvailableLowerLatin,
            IsEditIsAvailableNumber = IsEditIsAvailableNumber,
            IsEditIsAvailableSpecialSymbols = IsEditIsAvailableSpecialSymbols,
            IsEditIsAvailableUpperLatin = IsEditIsAvailableUpperLatin,
            IsEditKey = IsEditKey,
            IsEditLength = IsEditLength,
            IsEditLogin = IsEditLogin,
            IsEditName = IsEditName,
            IsEditRegex = IsEditRegex,
            Login = Login,
            IsEditType = IsEditType,
            Key = Key,
            Length = Length,
            Name = Name,
            Regex = Regex,
            Type = Type,
        };
    }

    private ValidationError[] ValidateAvailableCharacters()
    {
        return !IsAvailableLowerLatin
         && !IsAvailableNumber
         && !IsAvailableSpecialSymbols
         && !IsAvailableUpperLatin
         && CustomAvailableCharacters.IsNullOrWhiteSpace()
                ? [new PropertyEmptyValidationError("AvailableCharacters"),]
                : [];
    }

    private ValidationError[] ValidateLength()
    {
        return Length == 0 ? [new PropertyZeroValidationError(nameof(Length)),] : [];
    }

    private ValidationError[] ValidateKey()
    {
        return string.IsNullOrWhiteSpace(Key) ? [new PropertyEmptyValidationError(nameof(Key)),] : [];
    }

    private ValidationError[] ValidateLogin()
    {
        return string.IsNullOrWhiteSpace(Login) ? [new PropertyEmptyValidationError(nameof(Login)),] : [];
    }

    private ValidationError[] ValidateName()
    {
        return string.IsNullOrWhiteSpace(Name) ? [new PropertyEmptyValidationError(nameof(Name)),] : [];
    }

    private void InitValidation(ValidationMode validationMode)
    {
        SetValidation(nameof(Name),
            () =>
                validationMode switch
                {
                    ValidationMode.ValidateAll => ValidateName(),
                    ValidationMode.ValidateOnlyEdited => IsEditName ? ValidateName() : [],
                    _ => throw new ArgumentOutOfRangeException(nameof(validationMode), validationMode, null),
                }
        );

        SetValidation(nameof(Login),
            () => validationMode switch
            {
                ValidationMode.ValidateAll => ValidateLogin(),
                ValidationMode.ValidateOnlyEdited => IsEditLogin ? ValidateLogin() : [],
                _ => throw new ArgumentOutOfRangeException(nameof(validationMode), validationMode, null),
            });

        SetValidation(nameof(Key), () => validationMode switch
        {
            ValidationMode.ValidateAll => ValidateKey(),
            ValidationMode.ValidateOnlyEdited => IsEditKey ? ValidateKey() : [],
            _ => throw new ArgumentOutOfRangeException(nameof(validationMode), validationMode, null),
        });

        SetValidation(nameof(Length), () => validationMode switch
        {
            ValidationMode.ValidateAll => ValidateLength(),
            ValidationMode.ValidateOnlyEdited => IsEditLength ? ValidateLength() : [],
            _ => throw new ArgumentOutOfRangeException(nameof(validationMode), validationMode, null),
        });

        SetValidation(nameof(CustomAvailableCharacters),
            () => validationMode switch
            {
                ValidationMode.ValidateAll => ValidateAvailableCharacters(),
                ValidationMode.ValidateOnlyEdited => IsEditCustomAvailableCharacters || IsEditIsAvailableLowerLatin
                 || IsEditIsAvailableNumber || IsEditIsAvailableSpecialSymbols || IsEditIsAvailableUpperLatin
                        ? ValidateAvailableCharacters()
                        : [],
                _ => throw new ArgumentOutOfRangeException(nameof(validationMode), validationMode, null),
            });
    }
}