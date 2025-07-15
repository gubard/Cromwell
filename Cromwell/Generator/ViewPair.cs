namespace Cromwell.Generator;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class ViewPair : Attribute
{
    public ViewPair(Type view, Type viewModel)
    {
    }
}