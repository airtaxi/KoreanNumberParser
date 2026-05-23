using System;

namespace KoreanNumberParser;

/// <summary>
/// The exception that is thrown when a Korean number string cannot be parsed.
/// </summary>
public class KoreanNumberParseException : FormatException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KoreanNumberParseException"/> class.
    /// </summary>
    public KoreanNumberParseException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KoreanNumberParseException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public KoreanNumberParseException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KoreanNumberParseException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public KoreanNumberParseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
