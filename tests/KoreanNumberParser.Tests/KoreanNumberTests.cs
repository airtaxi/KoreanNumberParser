using KoreanNumberParser;

namespace KoreanNumberParser.Tests;

public class KoreanNumberTests
{
    [Theory]
    [InlineData("10억4천3백만이십일", 1043000021L)]
    [InlineData("사천만5백", 40000500L)]
    [InlineData("4000만오백", 40000500L)]
    [InlineData("십억사천삼백만이십일", 1043000021L)]
    [InlineData("10억4천3백만이십1", 1043000021L)]
    [InlineData("1십억4천3백만2십1", 1043000021L)]
    [InlineData("+ 1,043,000,021", 1043000021L)]
    [InlineData("마이너스 사천만 오백", -40000500L)]
    [InlineData("스물하나", 21L)]
    [InlineData("마흔두", 42L)]
    [InlineData("열한", 11L)]
    [InlineData("스무만", 200000L)]
    public void ParseInt64ParsesSupportedKoreanNumberStrings(string text, long expectedValue)
    {
        Assert.Equal(expectedValue, KoreanNumber.ParseInt64(text));
    }

    [Theory]
    [InlineData("10억4천3백만이십일", 1043000021)]
    [InlineData("사천만5백", 40000500)]
    [InlineData("4000만오백", 40000500)]
    public void ParseInt32ParsesSupportedKoreanNumberStrings(string text, int expectedValue)
    {
        Assert.Equal(expectedValue, KoreanNumber.ParseInt32(text));
    }

    [Fact]
    public void ParseInt64ParsesLongBoundaryValues()
    {
        Assert.Equal(long.MaxValue, KoreanNumber.ParseInt64("구백이십이경삼천삼백칠십이조삼백육십팔억오천사백칠십칠만오천팔백칠"));
        Assert.Equal(long.MinValue, KoreanNumber.ParseInt64("마이너스구백이십이경삼천삼백칠십이조삼백육십팔억오천사백칠십칠만오천팔백팔"));
    }

    [Fact]
    public void ParseInt32ParsesIntBoundaryValues()
    {
        Assert.Equal(int.MaxValue, KoreanNumber.ParseInt32("이십일억사천칠백사십팔만삼천육백사십칠"));
        Assert.Equal(int.MinValue, KoreanNumber.ParseInt32("마이너스이십일억사천칠백사십팔만삼천육백사십팔"));
    }

    [Theory]
    [InlineData("사천만오백", KoreanNumberFormat.Hangul)]
    [InlineData("4000만500", KoreanNumberFormat.ArabicChunk)]
    [InlineData("4천만5백", KoreanNumberFormat.ArabicUnitDigit)]
    public void ToKoreanStringFormatsExampleValue(string expectedText, KoreanNumberFormat format)
    {
        Assert.Equal(expectedText, KoreanNumber.ToKoreanString(40000500, format));
    }

    [Theory]
    [InlineData(1043000021L, KoreanNumberFormat.Hangul, "십억사천삼백만이십일")]
    [InlineData(1043000021L, KoreanNumberFormat.ArabicChunk, "10억4300만21")]
    [InlineData(1043000021L, KoreanNumberFormat.ArabicUnitDigit, "1십억4천3백만2십1")]
    [InlineData(0L, KoreanNumberFormat.Hangul, "영")]
    [InlineData(0L, KoreanNumberFormat.ArabicChunk, "0")]
    [InlineData(0L, KoreanNumberFormat.ArabicUnitDigit, "0")]
    [InlineData(-1L, KoreanNumberFormat.Hangul, "마이너스일")]
    [InlineData(-1L, KoreanNumberFormat.ArabicChunk, "-1")]
    [InlineData(-1L, KoreanNumberFormat.ArabicUnitDigit, "-1")]
    public void ToKoreanStringFormatsSupportedModes(long value, KoreanNumberFormat format, string expectedText)
    {
        Assert.Equal(expectedText, KoreanNumber.ToKoreanString(value, format));
    }

    [Fact]
    public void TryParseMethodsReturnFalseForInvalidInput()
    {
        Assert.False(KoreanNumber.TryParseInt32("사과", out var intValue));
        Assert.False(KoreanNumber.TryParseInt64(null, out var longValue));
        Assert.Equal(0, intValue);
        Assert.Equal(0, longValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("사과")]
    [InlineData("백천")]
    [InlineData("만억")]
    [InlineData("십십")]
    public void ParseMethodsThrowParseExceptionForInvalidInput(string text)
    {
        Assert.Throws<KoreanNumberParseException>(() => KoreanNumber.ParseInt64(text));
    }

    [Fact]
    public void ParseMethodsThrowOverflowExceptionForOutOfRangeValues()
    {
        Assert.Throws<KoreanNumberOverflowException>(() => KoreanNumber.ParseInt64("구백이십이경삼천삼백칠십이조삼백육십팔억오천사백칠십칠만오천팔백팔"));
        Assert.Throws<KoreanNumberOverflowException>(() => KoreanNumber.ParseInt32("이십일억사천칠백사십팔만삼천육백사십팔"));
    }

    [Fact]
    public void ParseMethodsThrowArgumentNullExceptionForNullInput()
    {
        Assert.Throws<ArgumentNullException>(() => KoreanNumber.ParseInt64(null!));
    }
}
