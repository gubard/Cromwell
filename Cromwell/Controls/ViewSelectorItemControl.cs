using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Metadata;

namespace Cromwell.Controls;

public class ViewSelectorItemControl : TemplatedControl
{
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<ViewSelectorItemControl, object?>(nameof(Content));

    public static readonly StyledProperty<object?> StateProperty =
        AvaloniaProperty.Register<ViewSelectorItemControl, object?>(
            nameof(State),
            defaultBindingMode: BindingMode.TwoWay
        );

    private ContentControl? contentControl;
    private object? lastState;

    public object? State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    [Content]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        contentControl = e.NameScope.Find<ContentControl>("PART_ContentControl");
        base.OnApplyTemplate(e);
        Update(lastState);
    }

    public void Update(object? currentState)
    {
        lastState = currentState;

        if (contentControl is null)
        {
            return;
        }

        if (currentState is null && State is null)
        {
            if (!contentControl.IsVisible)
            {
                contentControl.IsVisible = true;
            }

            return;
        }

        if (currentState is null || State is null || !currentState.Equals(State))
        {
            if (contentControl.IsVisible)
            {
                contentControl.IsVisible = false;
            }

            return;
        }

        if (!contentControl.IsVisible)
        {
            contentControl.IsVisible = true;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == StateProperty)
        {
            Update(lastState);
        }
    }
}