namespace Cromwell.Generator;

[AttributeUsage(AttributeTargets.Assembly)]
public class ViewPair : Attribute
{
    public ViewPair(Type view, Type viewModel)
    {
        View = view;
        ViewModel = viewModel;
    }

    public Type View { get; set; }
    public Type ViewModel { get; set; }
}