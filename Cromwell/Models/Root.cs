namespace Cromwell.Models;

public class Root
{
    public static readonly Root Instance = new();
    public static readonly IEnumerable<Root> IEnumerableInstance = [Instance];
    
    private Root()
    {
    }
}