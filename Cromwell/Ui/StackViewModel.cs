using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell.Models;

namespace Cromwell.Ui;

public partial class StackViewModel : ViewModelBase
{
    private readonly Stack<object> _views = new();
    [ObservableProperty] private object? _currentView;

    public void PushView(object view)
    {
        CurrentView = view;
        _views.Push(view);
    }

    public void PopView()
    {
        if (_views.Count == 0)
        {
            CurrentView = null;

            return;
        }

        _views.Pop();

        if (_views.Count == 0)
        {
            CurrentView = null;

            return;
        }

        CurrentView = _views.Peek();
    }
}