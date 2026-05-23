namespace KoreanNumberParser;

/// <summary>
/// Specifies how numeric values are formatted as Korean number strings.
/// </summary>
public enum KoreanNumberFormat
{
    /// <summary>
    /// Formats every digit and unit with Korean number words, such as <c>사천만오백</c>.
    /// </summary>
    Hangul,

    /// <summary>
    /// Formats each four-digit large-unit group with Arabic digits, such as <c>4000만500</c>.
    /// </summary>
    ArabicChunk,

    /// <summary>
    /// Formats digits before Korean units with Arabic digits, such as <c>4천만5백</c>.
    /// </summary>
    ArabicUnitDigit
}
