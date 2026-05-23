using System;
using System.Globalization;
using System.Text;

namespace KoreanNumberParser;

/// <summary>
/// Parses Korean number strings and formats integer values as Korean number strings.
/// </summary>
public static class KoreanNumber
{
    private const ulong TenThousand = 10000;
    private const ulong LongMaximumValue = 9223372036854775807;
    private const ulong LongMinimumAbsoluteValue = 9223372036854775808;

    private static readonly NumberToken[] s_nativeTens =
    [
        new("아흔", 90),
        new("여든", 80),
        new("일흔", 70),
        new("예순", 60),
        new("쉰", 50),
        new("마흔", 40),
        new("서른", 30),
        new("스물", 20),
        new("스무", 20),
        new("열", 10)
    ];

    private static readonly NumberToken[] s_nativeOnes =
    [
        new("다섯", 5),
        new("여섯", 6),
        new("일곱", 7),
        new("여덟", 8),
        new("아홉", 9),
        new("하나", 1),
        new("둘", 2),
        new("셋", 3),
        new("넷", 4),
        new("한", 1),
        new("두", 2),
        new("세", 3),
        new("네", 4)
    ];

    private static readonly string[] s_largeUnitNames = ["", "만", "억", "조", "경"];
    private static readonly string[] s_digitNames = ["영", "일", "이", "삼", "사", "오", "육", "칠", "팔", "구"];

    /// <summary>
    /// Parses a Korean number string into a 32-bit signed integer.
    /// </summary>
    /// <param name="text">The Korean number string to parse.</param>
    /// <returns>The parsed 32-bit signed integer.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
    /// <exception cref="KoreanNumberParseException"><paramref name="text"/> is not a valid Korean number string.</exception>
    /// <exception cref="KoreanNumberOverflowException">The parsed value is outside the range of <see cref="int"/>.</exception>
    public static int ParseInt32(string text)
    {
        var value = ParseInt64(text);
        if (value < int.MinValue || value > int.MaxValue) throw CreateOverflowException(text, nameof(Int32));
        return (int)value;
    }

    /// <summary>
    /// Parses a Korean number string into a 64-bit signed integer.
    /// </summary>
    /// <param name="text">The Korean number string to parse.</param>
    /// <returns>The parsed 64-bit signed integer.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
    /// <exception cref="KoreanNumberParseException"><paramref name="text"/> is not a valid Korean number string.</exception>
    /// <exception cref="KoreanNumberOverflowException">The parsed value is outside the range of <see cref="long"/>.</exception>
    public static long ParseInt64(string text)
    {
        if (text is null) throw new ArgumentNullException(nameof(text));

        var signedValue = ParseSignedAbsoluteValue(text);
        if (signedValue.IsNegative)
        {
            if (signedValue.AbsoluteValue == LongMinimumAbsoluteValue) return long.MinValue;
            if (signedValue.AbsoluteValue <= LongMaximumValue) return -(long)signedValue.AbsoluteValue;
            throw CreateOverflowException(text, nameof(Int64));
        }

        if (signedValue.AbsoluteValue <= LongMaximumValue) return (long)signedValue.AbsoluteValue;
        throw CreateOverflowException(text, nameof(Int64));
    }

    /// <summary>
    /// Tries to parse a Korean number string into a 32-bit signed integer.
    /// </summary>
    /// <param name="text">The Korean number string to parse.</param>
    /// <param name="value">When this method returns, contains the parsed value if parsing succeeded; otherwise, zero.</param>
    /// <returns><see langword="true"/> if parsing succeeded; otherwise, <see langword="false"/>.</returns>
    public static bool TryParseInt32(string? text, out int value)
    {
        try
        {
            value = ParseInt32(text!);
            return true;
        }
        catch (ArgumentNullException)
        {
            value = 0;
            return false;
        }
        catch (KoreanNumberParseException)
        {
            value = 0;
            return false;
        }
        catch (KoreanNumberOverflowException)
        {
            value = 0;
            return false;
        }
    }

    /// <summary>
    /// Tries to parse a Korean number string into a 64-bit signed integer.
    /// </summary>
    /// <param name="text">The Korean number string to parse.</param>
    /// <param name="value">When this method returns, contains the parsed value if parsing succeeded; otherwise, zero.</param>
    /// <returns><see langword="true"/> if parsing succeeded; otherwise, <see langword="false"/>.</returns>
    public static bool TryParseInt64(string? text, out long value)
    {
        try
        {
            value = ParseInt64(text!);
            return true;
        }
        catch (ArgumentNullException)
        {
            value = 0;
            return false;
        }
        catch (KoreanNumberParseException)
        {
            value = 0;
            return false;
        }
        catch (KoreanNumberOverflowException)
        {
            value = 0;
            return false;
        }
    }

    /// <summary>
    /// Formats a 32-bit signed integer as a Korean number string.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <param name="format">The Korean number output format.</param>
    /// <returns>The formatted Korean number string.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not a defined <see cref="KoreanNumberFormat"/> value.</exception>
    public static string ToKoreanString(int value, KoreanNumberFormat format = KoreanNumberFormat.Hangul) => ToKoreanString((long)value, format);

    /// <summary>
    /// Formats a 64-bit signed integer as a Korean number string.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <param name="format">The Korean number output format.</param>
    /// <returns>The formatted Korean number string.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not a defined <see cref="KoreanNumberFormat"/> value.</exception>
    public static string ToKoreanString(long value, KoreanNumberFormat format = KoreanNumberFormat.Hangul)
    {
        var isNegative = value < 0;
        var absoluteValue = GetAbsoluteValue(value);

        if (absoluteValue == 0) return format == KoreanNumberFormat.Hangul ? "영" : format == KoreanNumberFormat.ArabicChunk || format == KoreanNumberFormat.ArabicUnitDigit ? "0" : throw new ArgumentOutOfRangeException(nameof(format), format, "The Korean number format is not supported.");

        var formattedValue = format switch
        {
            KoreanNumberFormat.Hangul => FormatHangulValue(absoluteValue),
            KoreanNumberFormat.ArabicChunk => FormatArabicChunkValue(absoluteValue),
            KoreanNumberFormat.ArabicUnitDigit => FormatArabicUnitDigitValue(absoluteValue),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "The Korean number format is not supported.")
        };

        if (!isNegative) return formattedValue;
        return format == KoreanNumberFormat.Hangul ? "마이너스" + formattedValue : "-" + formattedValue;
    }

    private static SignedAbsoluteValue ParseSignedAbsoluteValue(string text)
    {
        var normalizedText = NormalizeText(text);
        if (normalizedText.Length == 0) throw CreateParseException(text);

        var isNegative = false;
        if (normalizedText[0] == '-' || normalizedText.StartsWith("마이너스", StringComparison.Ordinal))
        {
            isNegative = true;
            normalizedText = normalizedText[0] == '-' ? normalizedText.Substring(1) : normalizedText.Substring("마이너스".Length);
        }
        else if (normalizedText[0] == '+' || normalizedText.StartsWith("플러스", StringComparison.Ordinal))
        {
            normalizedText = normalizedText[0] == '+' ? normalizedText.Substring(1) : normalizedText.Substring("플러스".Length);
        }

        if (normalizedText.Length == 0) throw CreateParseException(text);

        var absoluteValue = ParseAbsoluteValue(normalizedText, text);
        if (absoluteValue > LongMinimumAbsoluteValue) throw CreateOverflowException(text, nameof(Int64));
        return new SignedAbsoluteValue(isNegative, absoluteValue);
    }

    private static string NormalizeText(string text)
    {
        var builder = new StringBuilder(text.Length);
        foreach (var character in text)
        {
            if (character == ',' || char.IsWhiteSpace(character)) continue;
            builder.Append(character);
        }

        return builder.ToString();
    }

    private static ulong ParseAbsoluteValue(string normalizedText, string originalText)
    {
        var totalValue = 0UL;
        var previousLargeUnitOrder = 5;
        var currentIndex = 0;

        while (currentIndex < normalizedText.Length)
        {
            var largeUnitIndex = FindNextLargeUnitIndex(normalizedText, currentIndex);
            if (largeUnitIndex < 0)
            {
                var lastSectionValue = ParseSectionValue(normalizedText.Substring(currentIndex), false, originalText);
                totalValue = AddChecked(totalValue, lastSectionValue, originalText);
                return totalValue;
            }

            var largeUnit = normalizedText[largeUnitIndex];
            var largeUnitOrder = GetLargeUnitOrder(largeUnit);
            if (largeUnitOrder >= previousLargeUnitOrder) throw CreateParseException(originalText);

            var sectionText = normalizedText.Substring(currentIndex, largeUnitIndex - currentIndex);
            var sectionValue = ParseSectionValue(sectionText, true, originalText);
            var multipliedSectionValue = MultiplyChecked(sectionValue, GetLargeUnitMultiplier(largeUnitOrder), originalText);
            totalValue = AddChecked(totalValue, multipliedSectionValue, originalText);
            if (totalValue > LongMinimumAbsoluteValue) throw CreateOverflowException(originalText, nameof(Int64));

            previousLargeUnitOrder = largeUnitOrder;
            currentIndex = largeUnitIndex + 1;
        }

        return totalValue;
    }

    private static int FindNextLargeUnitIndex(string text, int startIndex)
    {
        for (var index = startIndex; index < text.Length; index++)
        {
            if (IsLargeUnit(text[index])) return index;
        }

        return -1;
    }

    private static ulong ParseSectionValue(string sectionText, bool isBeforeLargeUnit, string originalText)
    {
        if (sectionText.Length == 0) return isBeforeLargeUnit ? 1UL : 0UL;
        if (!ContainsSmallUnit(sectionText)) return ParseUnitlessValue(sectionText, originalText);

        var totalValue = 0UL;
        var previousSmallUnitMultiplier = 10000;
        var currentIndex = 0;

        while (currentIndex < sectionText.Length)
        {
            var numberStartIndex = currentIndex;
            var hasNumber = TryParseLeadingNumber(sectionText, currentIndex, out var numberValue, out var nextIndex);
            currentIndex = nextIndex;

            if (currentIndex < sectionText.Length && TryGetSmallUnitMultiplier(sectionText[currentIndex], out var smallUnitMultiplier))
            {
                var coefficient = hasNumber ? numberValue : 1UL;
                if (coefficient == 0 || coefficient > 9) throw CreateParseException(originalText);
                if (smallUnitMultiplier >= previousSmallUnitMultiplier) throw CreateParseException(originalText);
                totalValue = AddChecked(totalValue, coefficient * (ulong)smallUnitMultiplier, originalText);
                previousSmallUnitMultiplier = smallUnitMultiplier;
                currentIndex++;
                continue;
            }

            if (!hasNumber) throw CreateParseException(originalText);

            var remainderText = sectionText.Substring(numberStartIndex);
            var remainderValue = ParseUnitlessValue(remainderText, originalText);
            if (remainderValue >= (ulong)previousSmallUnitMultiplier) throw CreateParseException(originalText);
            totalValue = AddChecked(totalValue, remainderValue, originalText);
            currentIndex = sectionText.Length;
        }

        return totalValue;
    }

    private static bool ContainsSmallUnit(string text) => text.IndexOf('천') >= 0 || text.IndexOf('백') >= 0 || text.IndexOf('십') >= 0;

    private static ulong ParseUnitlessValue(string text, string originalText)
    {
        if (TryParseArabicValue(text, out var arabicValue)) return arabicValue;
        if (TryParseNativeValue(text, out var nativeValue)) return nativeValue;
        if (TryParseKoreanDigitSequence(text, out var digitSequenceValue)) return digitSequenceValue;
        throw CreateParseException(originalText);
    }

    private static bool TryParseArabicValue(string text, out ulong value)
    {
        value = 0;
        if (text.Length == 0) return false;

        foreach (var character in text)
        {
            if (!IsAsciiDigit(character)) return false;
            value = AppendDecimalDigitChecked(value, character - '0');
        }

        return true;
    }

    private static bool TryParseNativeValue(string text, out ulong value)
    {
        value = 0;
        if (TryMatchNumberToken(text, 0, s_nativeTens, out var tensValue, out var nextIndex))
        {
            if (nextIndex == text.Length)
            {
                value = (ulong)tensValue;
                return true;
            }

            if (TryMatchNumberToken(text, nextIndex, s_nativeOnes, out var onesValue, out var finalIndex) && finalIndex == text.Length)
            {
                value = (ulong)(tensValue + onesValue);
                return true;
            }
        }

        if (TryMatchNumberToken(text, 0, s_nativeOnes, out var oneValue, out var oneIndex) && oneIndex == text.Length)
        {
            value = (ulong)oneValue;
            return true;
        }

        return false;
    }

    private static bool TryParseKoreanDigitSequence(string text, out ulong value)
    {
        value = 0;
        if (text.Length == 0) return false;

        foreach (var character in text)
        {
            if (IsAsciiDigit(character))
            {
                value = AppendDecimalDigitChecked(value, character - '0');
                continue;
            }

            if (!TryGetKoreanDigitValue(character, out var digitValue)) return false;
            value = AppendDecimalDigitChecked(value, digitValue);
        }

        return true;
    }

    private static bool TryParseLeadingNumber(string text, int startIndex, out ulong value, out int nextIndex)
    {
        value = 0;
        nextIndex = startIndex;
        if (startIndex >= text.Length) return false;

        if (IsAsciiDigit(text[startIndex]))
        {
            while (nextIndex < text.Length && IsAsciiDigit(text[nextIndex]))
            {
                value = AppendDecimalDigitChecked(value, text[nextIndex] - '0');
                nextIndex++;
            }

            return true;
        }

        if (TryGetKoreanDigitValue(text[startIndex], out var digitValue))
        {
            value = (ulong)digitValue;
            nextIndex = startIndex + 1;
            return true;
        }

        if (TryMatchNumberToken(text, startIndex, s_nativeTens, out var nativeTensValue, out nextIndex))
        {
            value = (ulong)nativeTensValue;
            return true;
        }

        if (TryMatchNumberToken(text, startIndex, s_nativeOnes, out var nativeOneValue, out nextIndex))
        {
            value = (ulong)nativeOneValue;
            return true;
        }

        return false;
    }

    private static bool TryMatchNumberToken(string text, int startIndex, NumberToken[] tokens, out int value, out int nextIndex)
    {
        foreach (var token in tokens)
        {
            if (!StartsWith(text, startIndex, token.Text)) continue;
            value = token.Value;
            nextIndex = startIndex + token.Text.Length;
            return true;
        }

        value = 0;
        nextIndex = startIndex;
        return false;
    }

    private static bool StartsWith(string text, int startIndex, string value)
    {
        if (startIndex + value.Length > text.Length) return false;
        return string.CompareOrdinal(text, startIndex, value, 0, value.Length) == 0;
    }

    private static ulong AppendDecimalDigitChecked(ulong value, int digit)
    {
        if (value > (ulong.MaxValue - (ulong)digit) / 10) throw new KoreanNumberOverflowException("The Korean number is too large to parse.");
        return value * 10 + (ulong)digit;
    }

    private static ulong AddChecked(ulong leftValue, ulong rightValue, string originalText)
    {
        if (ulong.MaxValue - leftValue < rightValue) throw CreateOverflowException(originalText, nameof(Int64));
        return leftValue + rightValue;
    }

    private static ulong MultiplyChecked(ulong value, ulong multiplier, string originalText)
    {
        if (value != 0 && multiplier > ulong.MaxValue / value) throw CreateOverflowException(originalText, nameof(Int64));
        return value * multiplier;
    }

    private static bool TryGetKoreanDigitValue(char character, out int value)
    {
        switch (character)
        {
            case '영':
            case '공':
                value = 0;
                return true;
            case '일':
                value = 1;
                return true;
            case '이':
                value = 2;
                return true;
            case '삼':
                value = 3;
                return true;
            case '사':
                value = 4;
                return true;
            case '오':
                value = 5;
                return true;
            case '육':
                value = 6;
                return true;
            case '칠':
                value = 7;
                return true;
            case '팔':
                value = 8;
                return true;
            case '구':
                value = 9;
                return true;
            default:
                value = 0;
                return false;
        }
    }

    private static bool TryGetSmallUnitMultiplier(char character, out int multiplier)
    {
        switch (character)
        {
            case '십':
                multiplier = 10;
                return true;
            case '백':
                multiplier = 100;
                return true;
            case '천':
                multiplier = 1000;
                return true;
            default:
                multiplier = 0;
                return false;
        }
    }

    private static bool IsLargeUnit(char character) => character == '만' || character == '억' || character == '조' || character == '경';

    private static int GetLargeUnitOrder(char character)
    {
        switch (character)
        {
            case '만':
                return 1;
            case '억':
                return 2;
            case '조':
                return 3;
            case '경':
                return 4;
            default:
                throw new ArgumentOutOfRangeException(nameof(character), character, "The character is not a large Korean number unit.");
        }
    }

    private static ulong GetLargeUnitMultiplier(int order)
    {
        var multiplier = 1UL;
        for (var index = 0; index < order; index++) multiplier *= TenThousand;
        return multiplier;
    }

    private static bool IsAsciiDigit(char character) => character >= '0' && character <= '9';

    private static string FormatHangulValue(ulong value) => FormatGroupedValue(value, FormatHangulGroup);

    private static string FormatArabicChunkValue(ulong value)
    {
        var builder = new StringBuilder();
        var groups = SplitGroups(value);

        for (var index = groups.Length - 1; index >= 0; index--)
        {
            var groupValue = groups[index];
            if (groupValue == 0) continue;
            builder.Append(groupValue.ToString(CultureInfo.InvariantCulture));
            builder.Append(s_largeUnitNames[index]);
        }

        return builder.ToString();
    }

    private static string FormatArabicUnitDigitValue(ulong value) => FormatGroupedValue(value, FormatArabicUnitDigitGroup);

    private static string FormatGroupedValue(ulong value, Func<int, string> groupFormatter)
    {
        var builder = new StringBuilder();
        var groups = SplitGroups(value);

        for (var index = groups.Length - 1; index >= 0; index--)
        {
            var groupValue = groups[index];
            if (groupValue == 0) continue;
            builder.Append(groupFormatter(groupValue));
            builder.Append(s_largeUnitNames[index]);
        }

        return builder.ToString();
    }

    private static int[] SplitGroups(ulong value)
    {
        var groups = new int[5];
        var currentValue = value;
        for (var index = 0; index < groups.Length; index++)
        {
            groups[index] = (int)(currentValue % TenThousand);
            currentValue /= TenThousand;
        }

        return groups;
    }

    private static string FormatHangulGroup(int value)
    {
        var builder = new StringBuilder();
        AppendHangulDigitUnit(builder, value / 1000, "천");
        AppendHangulDigitUnit(builder, value / 100 % 10, "백");
        AppendHangulDigitUnit(builder, value / 10 % 10, "십");

        var ones = value % 10;
        if (ones > 0) builder.Append(s_digitNames[ones]);
        return builder.ToString();
    }

    private static void AppendHangulDigitUnit(StringBuilder builder, int digit, string unit)
    {
        if (digit == 0) return;
        if (digit > 1) builder.Append(s_digitNames[digit]);
        builder.Append(unit);
    }

    private static string FormatArabicUnitDigitGroup(int value)
    {
        var builder = new StringBuilder();
        AppendArabicDigitUnit(builder, value / 1000, "천");
        AppendArabicDigitUnit(builder, value / 100 % 10, "백");
        AppendArabicDigitUnit(builder, value / 10 % 10, "십");

        var ones = value % 10;
        if (ones > 0) builder.Append(ones.ToString(CultureInfo.InvariantCulture));
        return builder.ToString();
    }

    private static void AppendArabicDigitUnit(StringBuilder builder, int digit, string unit)
    {
        if (digit == 0) return;
        builder.Append(digit.ToString(CultureInfo.InvariantCulture));
        builder.Append(unit);
    }

    private static ulong GetAbsoluteValue(long value)
    {
        if (value == long.MinValue) return LongMinimumAbsoluteValue;
        return value < 0 ? (ulong)-value : (ulong)value;
    }

    private static KoreanNumberParseException CreateParseException(string text) => new KoreanNumberParseException($"'{text}' is not a valid Korean number string.");

    private static KoreanNumberOverflowException CreateOverflowException(string text, string targetTypeName) => new KoreanNumberOverflowException($"'{text}' is outside the range of {targetTypeName}.");

    private readonly struct NumberToken(string text, int value)
    {
        public string Text { get; } = text;
        public int Value { get; } = value;
    }

    private readonly struct SignedAbsoluteValue(bool isNegative, ulong absoluteValue)
    {
        public bool IsNegative { get; } = isNegative;
        public ulong AbsoluteValue { get; } = absoluteValue;
    }
}
