using Avalonia.Collections;
using Cromwell.Helpers;
using Cromwell.Models;

namespace Cromwell.Ui;

public class DialogViewModel : ViewModelBase
{
    public static readonly DialogViewModel DesignViewModel = new("Header", "Design mode", UiHelper.OkButton,
        new DialogButton("Button", UiHelper.EmptyCommand, DialogButtonType.Normal), UiHelper.CancelButton);

    public DialogViewModel(object header, object content, params Span<DialogButton> buttons)
    {
        Content = content;
        Header = header;
        Buttons = new AvaloniaList<DialogButton>(buttons.ToArray());
    }

    public object Header { get; }
    public object Content { get; }
    public IAvaloniaReadOnlyList<DialogButton> Buttons { get; }
}