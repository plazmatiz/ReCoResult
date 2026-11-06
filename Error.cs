using ReCoResult.Interfaces;

namespace ReCoResult;

/// <summary>
/// Represents a concrete application error containing a descriptive message and a categorized error code.
/// </summary>
public class Error : IError
{
    /// <summary>
    /// Gets the human-readable explanation of the error.
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// Gets the categorized classification or domain code of the error.
    /// </summary>
    public ErrorCode ErrorCode { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class with a message and an optional error code.
    /// </summary>
    /// <param name="message">The description of the error.</param>
    /// <param name="errorCode">The code classifying the error. Defaults to <see cref="ErrorCode.Unexpected"/>.</param>
    public Error(string message, ErrorCode errorCode = ErrorCode.Unexpected)
    {
        Message = message;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates an <see cref="Error"/> instance representing an internal server or unexpected error.
    /// </summary>
    /// <param name="message">The description of the internal error.</param>
    /// <returns>A new <see cref="Error"/> instance initialized with the provided message.</returns>
    public static Error Internal(string message) => new(message);

    /// <summary>
    /// Returns the string representation of the error, which corresponds to the <see cref="Message"/>.
    /// </summary>
    /// <returns>The error message.</returns>
    public override string ToString() => Message;
}