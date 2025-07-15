namespace Cromwell.Models;

public class PropertyZeroValidationError : PropertyValidationError
{
    public PropertyZeroValidationError(string propertyName) : base(propertyName)
    {
    }
}

public class PropertyEmptyValidationError : PropertyValidationError
{
    public PropertyEmptyValidationError(string propertyName) : base(propertyName)
    {
    }
}

public abstract class PropertyValidationError : ValidationError
{
    protected PropertyValidationError(string propertyName)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; }
}