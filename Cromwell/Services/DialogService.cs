using Cromwell.Ui;
using DialogHostAvalonia;

namespace Cromwell.Services;

public interface IDialogService
{
    Task ShowMessageBoxAsync(DialogViewModel dialog);
    void CloseMessageBox();
}

public class DialogService : IDialogService
{
    private static readonly StackViewModel StackViewModel = new();
    private static readonly Stack<TaskCompletionSource> TaskStack = new();

    public Task ShowMessageBoxAsync(DialogViewModel dialog)
    {
        var showDialog = StackViewModel.CurrentView is null;
        StackViewModel.PushView(dialog);

        if (showDialog)
        {
            return DialogHost.Show((object)StackViewModel, "MessageBox");
        }

        var taskCompletionSource = new TaskCompletionSource();
        TaskStack.Push(taskCompletionSource);

        return taskCompletionSource.Task;
    }

    public void CloseMessageBox()
    {
        StackViewModel.PopView();

        if (StackViewModel.CurrentView is null)
        {
            DialogHost.Close("MessageBox");
        }
        else
        {
            TaskStack.Pop().SetResult();
        }
    }
}