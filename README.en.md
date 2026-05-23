# KoreanNumberParser

[![NuGet](https://img.shields.io/nuget/v/KoreanNumberParser.svg)](https://www.nuget.org/packages/KoreanNumberParser)
[![NuGet downloads](https://img.shields.io/nuget/dt/KoreanNumberParser.svg)](https://www.nuget.org/packages/KoreanNumberParser)
[![Pack and Publish](https://github.com/airtaxi/KoreanNumberParser/actions/workflows/pack-and-publish.yml/badge.svg)](https://github.com/airtaxi/KoreanNumberParser/actions/workflows/pack-and-publish.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

🌐 English | [한국어](README.md)

KoreanNumberParser is a .NET library for parsing Korean number strings into `int` and `long` values, and for formatting integer values back into Korean number strings.

It supports mixed Korean and Arabic-number expressions such as `10억4천3백만이십일`, `사천만5백`, and `4000만오백`.

## Install

```powershell
dotnet add package KoreanNumberParser
```

## Usage

```csharp
using KoreanNumberParser;

var value1 = KoreanNumber.ParseInt64("10억4천3백만이십일");
var value2 = KoreanNumber.ParseInt32("사천만5백");
var value3 = KoreanNumber.ParseInt64("4000만오백");

Console.WriteLine(value1); // 1043000021
Console.WriteLine(value2); // 40000500
Console.WriteLine(value3); // 40000500
```

## Formatting

```csharp
using KoreanNumberParser;

var hangul = KoreanNumber.ToKoreanString(40000500);
var arabicChunk = KoreanNumber.ToKoreanString(40000500, KoreanNumberFormat.ArabicChunk);
var arabicUnitDigit = KoreanNumber.ToKoreanString(40000500, KoreanNumberFormat.ArabicUnitDigit);

Console.WriteLine(hangul); // 사천만오백
Console.WriteLine(arabicChunk); // 4000만500
Console.WriteLine(arabicUnitDigit); // 4천만5백
```

## Supported Input

- `int` and `long` parsing and formatting.
- Korean digit words: `영`, `공`, `일`, `이`, `삼`, `사`, `오`, `육`, `칠`, `팔`, `구`.
- Korean units: `십`, `백`, `천`, `만`, `억`, `조`, `경`.
- Native Korean number words such as `하나`, `둘`, `셋`, `넷`, `스물`, `서른`, and `마흔`.
- `-`, `+`, `마이너스`, `플러스`, whitespace, and commas.
- Mixed Arabic digits and Korean units.

Native Korean number words are supported for parsing only. Formatting always uses Sino-Korean number words.

## Exceptions

- `KoreanNumberParseException`: thrown when the input cannot be interpreted as a Korean number.
- `KoreanNumberOverflowException`: thrown when the parsed value is outside the requested numeric type range.
- `ArgumentNullException`: thrown when `null` is passed to `ParseInt32` or `ParseInt64`.

`TryParseInt32` and `TryParseInt64` return `false` instead of throwing when parsing fails.

## License

KoreanNumberParser is licensed under the [MIT License](LICENSE).

## Author

Created by [Howon Lee (airtaxi)](https://github.com/airtaxi).

Built with help from OpenAI Codex.
