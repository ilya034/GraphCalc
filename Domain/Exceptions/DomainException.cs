namespace GraphCalc.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public string ErrorCode { get; protected set; }

    protected DomainException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}
