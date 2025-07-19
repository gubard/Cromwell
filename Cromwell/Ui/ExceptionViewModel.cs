using Avalonia.Collections;
using Cromwell.Models;

namespace Cromwell.Ui;

public class ExceptionViewModel : ViewModelBase
{
    private readonly AvaloniaList<Exception> _exceptions;

    public ExceptionViewModel(params Span<Exception> exceptions)
    {
        _exceptions = new(exceptions.ToArray());
    }

    public IAvaloniaReadOnlyList<Exception> Exceptions => _exceptions;
}