namespace SGE.Core.Exceptions;

/// <summary>
///     Base exception class for all SGE system exceptions.
/// </summary>
public class SgeException : Exception
{
    public SgeException(string message, string errorCode, int statusCode)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    public string ErrorCode { get; }
    public int StatusCode { get; }
}