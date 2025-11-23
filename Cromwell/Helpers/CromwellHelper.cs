using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Cromwell.Ui;
using Gaia.Extensions;
using Inanna.Helpers;

namespace Cromwell.Helpers;

public static class CromwellHelper
{
    public static readonly AttachedProperty<bool> IsDragHandleProperty =
        AvaloniaProperty.RegisterAttached<InputElement, bool>("IsDragHandle", typeof(CromwellHelper));

    public static void SetIsDragHandle(InputElement element, bool value) =>
        element.SetValue(IsDragHandleProperty, value);

    public static bool GetIsDragHandle(InputElement element) => element.GetValue(IsDragHandleProperty);

    static CromwellHelper()
    {
        IsDragHandleProperty.Changed.AddClassHandler<Control, bool>((_, e) =>
        {
            if (e.Sender is not InputElement element)
            {
                return;
            }

            if (e.NewValue.GetValueOrDefault<bool>())
            {
                element.PointerPressed += DragOnPointerPressed;
            }
            else
            {
                element.PointerPressed -= DragOnPointerPressed;
            }
        });
    }


    public static async void DragOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not IDataContextProvider dataContextProvider)
        {
            return;
        }

        if (dataContextProvider.DataContext is not CredentialParametersViewModel credentialParametersViewModel)
        {
            return;
        }

        e.Handled = true;
        var dragData = new DataTransfer();
        var dataTransferItem = new DataTransferItem();

        dataTransferItem.Set(DataFormat.CreateBytesApplicationFormat(nameof(CredentialParametersViewModel)),
            credentialParametersViewModel.Id.ToByteArray());

        dragData.Add(dataTransferItem);
        var item = sender.As<ILogical>()?.GetLogicalAncestors().OfType<TreeViewItem>().FirstOrDefault().As<Visual>();

        if (item is null)
        {
            await TopLevelAssist.DoDragDropAsync(e, dragData, DragDropEffects.Move);
        }
        else
        {
            item.IsVisible = false;
            await TopLevelAssist.DoDragDropAsync(e, dragData, DragDropEffects.Move);
            item.IsVisible = true;
        }
    }
}