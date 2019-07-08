// Decompiled with JetBrains decompiler
// Type: xNet.Html
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace xNet
{
  public static class Html
  {
    private static readonly Dictionary<string, string> _htmlMnemonics = new Dictionary<string, string>()
    {
      {
        "apos",
        "'"
      },
      {
        "quot",
        "\""
      },
      {
        "amp",
        "&"
      },
      {
        "lt",
        "<"
      },
      {
        "gt",
        ">"
      }
    };

    public static string ReplaceEntities(this string str)
    {
      if (string.IsNullOrEmpty(str))
        return string.Empty;
      return new Regex("(\\&(?<text>\\w{1,4})\\;)|(\\&#(?<code>\\w{1,4})\\;)", RegexOptions.Compiled).Replace(str, (MatchEvaluator) (match =>
      {
        if (match.Groups["text"].Success)
        {
          string str1;
          if (Html._htmlMnemonics.TryGetValue(match.Groups["text"].Value, out str1))
            return str1;
        }
        else if (match.Groups["code"].Success)
          return ((char) int.Parse(match.Groups["code"].Value)).ToString();
        return match.Value;
      }));
    }

    public static string ReplaceUnicode(this string str)
    {
      if (string.IsNullOrEmpty(str))
        return string.Empty;
      return new Regex("\\\\u(?<code>[0-9a-f]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(str, (MatchEvaluator) (match => ((char) int.Parse(match.Groups["code"].Value, NumberStyles.HexNumber)).ToString()));
    }

    public static string Substring(
      this string str,
      string left,
      int startIndex,
      StringComparison comparsion = StringComparison.Ordinal)
    {
      if (string.IsNullOrEmpty(str))
        return string.Empty;
      if (left == null)
        throw new ArgumentNullException(nameof (left));
      if (left.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (left));
      if (startIndex < 0)
        throw ExceptionHelper.CanNotBeLess<int>(nameof (startIndex), 0);
      if (startIndex >= str.Length)
        throw new ArgumentOutOfRangeException(nameof (startIndex), Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
      int num = str.IndexOf(left, startIndex, comparsion);
      if (num == -1)
        return string.Empty;
      int startIndex1 = num + left.Length;
      int length = str.Length - startIndex1;
      return str.Substring(startIndex1, length);
    }

    public static string Substring(this string str, string left, StringComparison comparsion = StringComparison.Ordinal)
    {
      return str.Substring(left, 0, comparsion);
    }

    public static string Substring(
      this string str,
      string left,
      string right,
      int startIndex,
      StringComparison comparsion = StringComparison.Ordinal)
    {
      if (string.IsNullOrEmpty(str))
        return string.Empty;
      if (left == null)
        throw new ArgumentNullException(nameof (left));
      if (left.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (left));
      if (right == null)
        throw new ArgumentNullException(nameof (right));
      if (right.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (right));
      if (startIndex < 0)
        throw ExceptionHelper.CanNotBeLess<int>(nameof (startIndex), 0);
      if (startIndex >= str.Length)
        throw new ArgumentOutOfRangeException(nameof (startIndex), Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
      int num1 = str.IndexOf(left, startIndex, comparsion);
      if (num1 == -1)
        return string.Empty;
      int startIndex1 = num1 + left.Length;
      int num2 = str.IndexOf(right, startIndex1, comparsion);
      if (num2 == -1)
        return string.Empty;
      int length = num2 - startIndex1;
      return str.Substring(startIndex1, length);
    }

    public static string Substring(
      this string str,
      string left,
      string right,
      StringComparison comparsion = StringComparison.Ordinal)
    {
      return str.Substring(left, right, 0, comparsion);
    }

    public static string LastSubstring(
      this string str,
      string left,
      int startIndex,
      StringComparison comparsion = StringComparison.Ordinal)
    {
      if (string.IsNullOrEmpty(str))
        return string.Empty;
      if (left == null)
        throw new ArgumentNullException(nameof (left));
      if (left.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (left));
      if (startIndex < 0)
        throw ExceptionHelper.CanNotBeLess<int>(nameof (startIndex), 0);
      if (startIndex >= str.Length)
        throw new ArgumentOutOfRangeException(nameof (startIndex), Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
      int num = str.LastIndexOf(left, startIndex, comparsion);
      if (num == -1)
        return string.Empty;
      int startIndex1 = num + left.Length;
      int length = str.Length - startIndex1;
      return str.Substring(startIndex1, length);
    }

    public static string LastSubstring(this string str, string left, StringComparison comparsion = StringComparison.Ordinal)
    {
      if (string.IsNullOrEmpty(str))
        return string.Empty;
      return str.LastSubstring(left, str.Length - 1, comparsion);
    }

    public static string LastSubstring(
      this string str,
      string left,
      string right,
      int startIndex,
      StringComparison comparsion = StringComparison.Ordinal)
    {
      if (string.IsNullOrEmpty(str))
        return string.Empty;
      if (left == null)
        throw new ArgumentNullException(nameof (left));
      if (left.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (left));
      if (right == null)
        throw new ArgumentNullException(nameof (right));
      if (right.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (right));
      if (startIndex < 0)
        throw ExceptionHelper.CanNotBeLess<int>(nameof (startIndex), 0);
      if (startIndex >= str.Length)
        throw new ArgumentOutOfRangeException(nameof (startIndex), Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
      int num1 = str.LastIndexOf(left, startIndex, comparsion);
      if (num1 == -1)
        return string.Empty;
      int startIndex1 = num1 + left.Length;
      int num2 = str.IndexOf(right, startIndex1, comparsion);
      if (num2 == -1)
      {
        if (num1 == 0)
          return string.Empty;
        return str.LastSubstring(left, right, num1 - 1, comparsion);
      }
      int length = num2 - startIndex1;
      return str.Substring(startIndex1, length);
    }

    public static string LastSubstring(
      this string str,
      string left,
      string right,
      StringComparison comparsion = StringComparison.Ordinal)
    {
      if (string.IsNullOrEmpty(str))
        return string.Empty;
      return str.LastSubstring(left, right, str.Length - 1, comparsion);
    }

    public static string[] Substrings(
      this string str,
      string left,
      string right,
      int startIndex,
      StringComparison comparsion = StringComparison.Ordinal)
    {
      if (string.IsNullOrEmpty(str))
        return new string[0];
      if (left == null)
        throw new ArgumentNullException(nameof (left));
      if (left.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (left));
      if (right == null)
        throw new ArgumentNullException(nameof (right));
      if (right.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (right));
      if (startIndex < 0)
        throw ExceptionHelper.CanNotBeLess<int>(nameof (startIndex), 0);
      if (startIndex >= str.Length)
        throw new ArgumentOutOfRangeException(nameof (startIndex), Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
      int startIndex1 = startIndex;
      List<string> stringList = new List<string>();
      while (true)
      {
        int num1 = str.IndexOf(left, startIndex1, comparsion);
        if (num1 != -1)
        {
          int startIndex2 = num1 + left.Length;
          int num2 = str.IndexOf(right, startIndex2, comparsion);
          if (num2 != -1)
          {
            int length = num2 - startIndex2;
            stringList.Add(str.Substring(startIndex2, length));
            startIndex1 = num2 + right.Length;
          }
          else
            break;
        }
        else
          break;
      }
      return stringList.ToArray();
    }

    public static string[] Substrings(
      this string str,
      string left,
      string right,
      StringComparison comparsion = StringComparison.Ordinal)
    {
      return str.Substrings(left, right, 0, comparsion);
    }
  }
}
