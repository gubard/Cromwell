using Cromwell.Exceptions;

namespace Cromwell.Services;

public interface IDragAndDropService
{
    object? GetDataAndRelease();
    void SetData(object? data);
    bool IsDragging { get; }
}

public class DragAndDropService : IDragAndDropService
{
    private object? _data;

    public object? GetDataAndRelease()
    {
        if (!IsDragging)
        {
            throw new DataNotDraggingException();
        }

        var result = _data;
        _data = null;
        IsDragging = false;

        return result;
    }

    public void SetData(object? data)
    {
        if (IsDragging)
        {
            throw new DataDraggingException();
        }

        _data = data;
        IsDragging = true;
    }

    public bool IsDragging { get; private set; }
}