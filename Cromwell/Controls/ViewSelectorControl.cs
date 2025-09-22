using System.Collections.Specialized;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Metadata;

namespace Cromwell.Controls;

public class ViewSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<ViewSelectorItemControl>> ItemsProperty =
        AvaloniaProperty.Register<ViewSelectorControl, IEnumerable<ViewSelectorItemControl>>(nameof(Items));

    public static readonly StyledProperty<object?> StateProperty =
        AvaloniaProperty.Register<ViewSelectorControl, object?>(nameof(State), defaultBindingMode: BindingMode.TwoWay);

    private IAvaloniaReadOnlyList<ViewSelectorItemControl>? itemsNotify;

    private Panel? panel;

    public ViewSelectorControl()
    {
        Items = new AvaloniaList<ViewSelectorItemControl>();
    }

    public object? State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    [Content]
    public IEnumerable<ViewSelectorItemControl> Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        panel = e.NameScope.Find<Panel>("PART_Panel");
        base.OnApplyTemplate(e);

        if (panel is null)
        {
            return;
        }

        panel.Children.AddRange(Items);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == StateProperty)
        {
            UpdateItems();
        }
        else if (change.Property == ItemsProperty)
        {
            if (itemsNotify is not null)
            {
                itemsNotify.CollectionChanged -= ItemsCollectionChanged;

                foreach (var i in itemsNotify)
                {
                    i.PropertyChanged -= OnItemStateChanged;
                }
            }

            if (change.NewValue is IAvaloniaReadOnlyList<ViewSelectorItemControl> notify)
            {
                itemsNotify = notify;

                if (itemsNotify is not null)
                {
                    itemsNotify.CollectionChanged += ItemsCollectionChanged;

                    foreach (var i in itemsNotify)
                    {
                        i.PropertyChanged += OnItemStateChanged;
                    }
                }
            }

            UpdateItems();
        }
    }

    private void OnItemStateChanged(object? sender, AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == ViewSelectorItemControl.StateProperty)
        {
            UpdateItems();
        }
    }

    private void UpdateItems()
    {
        foreach (var item in Items)
        {
            item.Update(State);
        }
    }

    private void ItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (var i in e.OldItems.OfType<ViewSelectorItemControl>())
            {
                i.PropertyChanged -= OnItemStateChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (var i in e.NewItems.OfType<ViewSelectorItemControl>())
            {
                i.PropertyChanged += OnItemStateChanged;
            }
        }

        UpdateItems();
    }
}