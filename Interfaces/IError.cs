namespace ReCo.Result.Interfaces;

/// <summary>
/// Defines the contract for error objects used within the result pattern.
/// </summary>
public interface IError
{
    /// <summary>
    /// Gets the descriptive explanation of the error.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Gets the classification code of the error.
    /// </summary>
    ErrorCode ErrorCode { get; }
}