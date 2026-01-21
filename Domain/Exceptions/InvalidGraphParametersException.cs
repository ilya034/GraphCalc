namespace GraphCalc.Domain.Exceptions;

/// <summary>
/// Исключение при некорректных параметрах расчета графика
/// </summary>
public class InvalidGraphParametersException : DomainException
{
    public InvalidGraphParametersException(string message)
        : base(message, "INVALID_PARAMETERS")
    {
    }
}
