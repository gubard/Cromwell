using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Db;
using Cromwell.Generator;
using Cromwell.Models;
using Cromwell.Services;

namespace Cromwell.Ui;

[EditNotify]
public partial class EditCredentialViewModel : ViewModelBase
{
    private readonly ICredentialService _credentialService;
    private readonly IDialogService _dialogService;

    public EditCredentialViewModel(
        CredentialEntity entity,
        ICredentialService credentialService,
        IDialogService dialogService
    ) : this(entity.Id, credentialService, dialogService)
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

    public EditCredentialViewModel(Guid id, ICredentialService credentialService, IDialogService dialogService)
    {
        Id = id;
        _credentialService = credentialService;
        _dialogService = dialogService;

        SetValidation(nameof(Name),
            () => string.IsNullOrWhiteSpace(Name) ? [new PropertyEmptyValidationError(nameof(Name)),] : []);

        SetValidation(nameof(Login),
            () => string.IsNullOrWhiteSpace(Login) ? [new PropertyEmptyValidationError(nameof(Login)),] : []);

        SetValidation(nameof(Key),
            () => string.IsNullOrWhiteSpace(Key) ? [new PropertyEmptyValidationError(nameof(Key)),] : []);

        SetValidation(nameof(Length), () => Length == 0 ? [new PropertyZeroValidationError(nameof(Length)),] : []);
        Length = 512;
        Children = new();
    }

    public Guid Id { get; }
    public AvaloniaList<EditCredentialViewModel> Children { get; }

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

    [RelayCommand]
    private Task SaveAsync()
    {
        return WrapCommand(() => Task.CompletedTask);
    }

    [RelayCommand]
    private Task CreateAsync(CancellationToken cancellationToken)
    {
        return WrapCommand(async () =>
        {
            await _credentialService.AddAsync(new()
            {
                Id = Id,
                Name = Name,
                Login = Login,
                Key = Key,
                IsAvailableUpperLatin = IsAvailableUpperLatin,
                IsAvailableLowerLatin = IsAvailableLowerLatin,
                IsAvailableNumber = IsAvailableNumber,
                IsAvailableSpecialSymbols = IsAvailableSpecialSymbols,
                CustomAvailableCharacters = CustomAvailableCharacters,
                Length = Length,
                Regex = Regex,
                Type = Type,
                OrderIndex = 0, // You may want to set this based on your business logic
                ParentId = null, // Set this if this credential has a parent
            }, cancellationToken);

            _dialogService.CloseMessageBox();
        });
    }
}