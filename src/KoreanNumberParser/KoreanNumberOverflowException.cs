using System;

namespace KoreanNumberParser;

/// <summary>
/// The exception that is thrown when a parsed Korean number is outside the requested numeric range.
/// </summary>
public class KoreanNumberOverflowException : OverflowException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KoreanNumberOverflowException"/> class.
    /// </summary>
    public KoreanNumberOverflowException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KoreanNumberOverflowException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public KoreanNumberOverflowException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KoreanNumberOverflowException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public KoreanNumberOverflowException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
