# KoreanNumberParser

[![NuGet](https://img.shields.io/nuget/v/KoreanNumberParser.svg)](https://www.nuget.org/packages/KoreanNumberParser)
[![NuGet downloads](https://img.shields.io/nuget/dt/KoreanNumberParser.svg)](https://www.nuget.org/packages/KoreanNumberParser)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

🌐 [English](README.en.md) | 한국어

KoreanNumberParser는 한국어 숫자 문자열을 `int`와 `long`으로 변환하고, 정수 값을 다시 한국어 숫자 문자열로 변환하는 .NET 라이브러리입니다.

`10억4천3백만이십일`, `사천만5백`, `4000만오백`처럼 한자어 수사와 아라비아 숫자가 섞인 표현을 처리할 수 있습니다.

## 설치

```powershell
dotnet add package KoreanNumberParser
```

## 사용법

```csharp
using KoreanNumberParser;

var value1 = KoreanNumber.ParseInt64("10억4천3백만이십일");
var value2 = KoreanNumber.ParseInt32("사천만5백");
var value3 = KoreanNumber.ParseInt64("4000만오백");

Console.WriteLine(value1); // 1043000021
Console.WriteLine(value2); // 40000500
Console.WriteLine(value3); // 40000500
```

## 문자열로 변환

```csharp
using KoreanNumberParser;

var hangul = KoreanNumber.ToKoreanString(40000500);
var arabicChunk = KoreanNumber.ToKoreanString(40000500, KoreanNumberFormat.ArabicChunk);
var arabicUnitDigit = KoreanNumber.ToKoreanString(40000500, KoreanNumberFormat.ArabicUnitDigit);

Console.WriteLine(hangul); // 사천만오백
Console.WriteLine(arabicChunk); // 4000만500
Console.WriteLine(arabicUnitDigit); // 4천만5백
```

## 지원 범위

- `int`, `long` 파싱과 포맷팅.
- `영`, `공`, `일`, `이`, `삼`, `사`, `오`, `육`, `칠`, `팔`, `구`.
- `십`, `백`, `천`, `만`, `억`, `조`, `경`.
- `하나`, `둘`, `셋`, `넷`, `스물`, `서른`, `마흔` 같은 고유어 수사 입력.
- `-`, `+`, `마이너스`, `플러스`, 공백, 쉼표.
- 아라비아 숫자와 한국어 단위가 섞인 입력.

고유어 수사는 입력 파싱에서만 지원합니다. 출력은 항상 한자어 수사 기반입니다.

## 예외

- `KoreanNumberParseException`: 입력 문자열을 한국어 숫자로 해석할 수 없을 때 발생합니다.
- `KoreanNumberOverflowException`: 파싱한 값이 요청한 타입의 범위를 벗어날 때 발생합니다.
- `ArgumentNullException`: `ParseInt32` 또는 `ParseInt64`에 `null`을 전달했을 때 발생합니다.

`TryParseInt32`, `TryParseInt64`는 실패 시 예외를 던지지 않고 `false`를 반환합니다.

## 라이선스

KoreanNumberParser는 [MIT 라이선스](LICENSE)로 배포됩니다.

## 제작자

[이호원 (airtaxi)](https://github.com/airtaxi)이 만들었습니다.

OpenAI Codex의 도움을 받아 제작되었습니다.
