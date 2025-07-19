using System.Windows.Input;

namespace Cromwell.Models;

public class DialogButton
{
    public DialogButton(object content, ICommand command, DialogButtonType type)
    {
        Content = content;
        Command = command;

        switch (type)
        {
            case DialogButtonType.Normal:
                break;
            case DialogButtonType.Primary:
                IsPrimary = true;

                break;
            case DialogButtonType.Danger:
                IsDanger = true;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public object Content { get; }
    public ICommand Command { get; }
    public bool IsPrimary { get; }
    public bool IsDanger { get; }
}