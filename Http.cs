// Decompiled with JetBrains decompiler
// Type: xNet.Http
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace xNet
{
  public static class Http
  {
    internal static readonly Dictionary<HttpHeader, string> Headers = new Dictionary<HttpHeader, string>()
    {
      {
        HttpHeader.Accept,
        "Accept"
      },
      {
        HttpHeader.AcceptCharset,
        "Accept-Charset"
      },
      {
        HttpHeader.AcceptLanguage,
        "Accept-Language"
      },
      {
        HttpHeader.AcceptDatetime,
        "Accept-Datetime"
      },
      {
        HttpHeader.CacheControl,
        "Cache-Control"
      },
      {
        HttpHeader.ContentType,
        "Content-Type"
      },
      {
        HttpHeader.Date,
        "Date"
      },
      {
        HttpHeader.Expect,
        "Expect"
      },
      {
        HttpHeader.From,
        "From"
      },
      {
        HttpHeader.IfMatch,
        "If-Match"
      },
      {
        HttpHeader.IfModifiedSince,
        "If-Modified-Since"
      },
      {
        HttpHeader.IfNoneMatch,
        "If-None-Match"
      },
      {
        HttpHeader.IfRange,
        "If-Range"
      },
      {
        HttpHeader.IfUnmodifiedSince,
        "If-Unmodified-Since"
      },
      {
        HttpHeader.MaxForwards,
        "Max-Forwards"
      },
      {
        HttpHeader.Pragma,
        "Pragma"
      },
      {
        HttpHeader.Range,
        "Range"
      },
      {
        HttpHeader.Referer,
        "Referer"
      },
      {
        HttpHeader.Upgrade,
        "Upgrade"
      },
      {
        HttpHeader.UserAgent,
        "User-Agent"
      },
      {
        HttpHeader.Via,
        "Via"
      },
      {
        HttpHeader.Warning,
        "Warning"
      },
      {
        HttpHeader.DNT,
        "DNT"
      },
      {
        HttpHeader.AccessControlAllowOrigin,
        "Access-Control-Allow-Origin"
      },
      {
        HttpHeader.AcceptRanges,
        "Accept-Ranges"
      },
      {
        HttpHeader.Age,
        "Age"
      },
      {
        HttpHeader.Allow,
        "Allow"
      },
      {
        HttpHeader.ContentEncoding,
        "Content-Encoding"
      },
      {
        HttpHeader.ContentLanguage,
        "Content-Language"
      },
      {
        HttpHeader.ContentLength,
        "Content-Length"
      },
      {
        HttpHeader.ContentLocation,
        "Content-Location"
      },
      {
        HttpHeader.ContentMD5,
        "Content-MD5"
      },
      {
        HttpHeader.ContentDisposition,
        "Content-Disposition"
      },
      {
        HttpHeader.ContentRange,
        "Content-Range"
      },
      {
        HttpHeader.ETag,
        "ETag"
      },
      {
        HttpHeader.Expires,
        "Expires"
      },
      {
        HttpHeader.LastModified,
        "Last-Modified"
      },
      {
        HttpHeader.Link,
        "Link"
      },
      {
        HttpHeader.Location,
        "Location"
      },
      {
        HttpHeader.P3P,
        "P3P"
      },
      {
        HttpHeader.Refresh,
        "Refresh"
      },
      {
        HttpHeader.RetryAfter,
        "Retry-After"
      },
      {
        HttpHeader.Server,
        "Server"
      },
      {
        HttpHeader.TransferEncoding,
        "Transfer-Encoding"
      }
    };
    public static readonly RemoteCertificateValidationCallback AcceptAllCertificationsCallback = new RemoteCertificateValidationCallback(Http.AcceptAllCertifications);
    public const string NewLine = "\r\n";
    [ThreadStatic]
    private static Random rand;

    private static Random Rand
    {
      get
      {
        if (Http.rand == null)
          Http.rand = new Random();
        return Http.rand;
      }
    }

    public static string UrlEncode(string str, Encoding encoding = null)
    {
      if (string.IsNullOrEmpty(str))
        return string.Empty;
      if (encoding == null)
        encoding = Encoding.UTF8;
      byte[] bytes1 = encoding.GetBytes(str);
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < bytes1.Length; ++index)
      {
        char c = (char) bytes1[index];
        if (c == ' ')
          ++num1;
        else if (!Http.IsUrlSafeChar(c))
          ++num2;
      }
      if (num1 == 0 && num2 == 0)
        return str;
      int num3 = 0;
      byte[] bytes2 = new byte[bytes1.Length + num2 * 2];
      for (int index1 = 0; index1 < bytes1.Length; ++index1)
      {
        char c = (char) bytes1[index1];
        if (Http.IsUrlSafeChar(c))
          bytes2[num3++] = bytes1[index1];
        else if (c == ' ')
        {
          bytes2[num3++] = (byte) 43;
        }
        else
        {
          byte[] numArray1 = bytes2;
          int index2 = num3;
          int num4 = index2 + 1;
          numArray1[index2] = (byte) 37;
          byte[] numArray2 = bytes2;
          int index3 = num4;
          int num5 = index3 + 1;
          int hex1 = (int) (byte) Http.IntToHex((int) bytes1[index1] >> 4 & 15);
          numArray2[index3] = (byte) hex1;
          byte[] numArray3 = bytes2;
          int index4 = num5;
          num3 = index4 + 1;
          int hex2 = (int) (byte) Http.IntToHex((int) bytes1[index1] & 15);
          numArray3[index4] = (byte) hex2;
        }
      }
      return Encoding.ASCII.GetString(bytes2);
    }

    public static string ToQueryString(
      IEnumerable<KeyValuePair<string, string>> parameters,
      bool dontEscape)
    {
      if (parameters == null)
        throw new ArgumentNullException(nameof (parameters));
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, string> parameter in parameters)
      {
        if (!string.IsNullOrEmpty(parameter.Key))
        {
          stringBuilder.Append(parameter.Key);
          stringBuilder.Append('=');
          if (dontEscape)
            stringBuilder.Append(parameter.Value);
          else
            stringBuilder.Append(Uri.EscapeDataString(parameter.Value ?? string.Empty));
          stringBuilder.Append('&');
        }
      }
      if (stringBuilder.Length != 0)
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
      return stringBuilder.ToString();
    }

    public static string ToPostQueryString(
      IEnumerable<KeyValuePair<string, string>> parameters,
      bool dontEscape,
      Encoding encoding = null)
    {
      if (parameters == null)
        throw new ArgumentNullException(nameof (parameters));
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, string> parameter in parameters)
      {
        if (!string.IsNullOrEmpty(parameter.Key))
        {
          stringBuilder.Append(parameter.Key);
          stringBuilder.Append('=');
          if (dontEscape)
            stringBuilder.Append(parameter.Value);
          else
            stringBuilder.Append(Http.UrlEncode(parameter.Value ?? string.Empty, encoding));
          stringBuilder.Append('&');
        }
      }
      if (stringBuilder.Length != 0)
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
      return stringBuilder.ToString();
    }

    public static string DetermineMediaType(string extension)
    {
      string str = "application/octet-stream";
      try
      {
        using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(extension))
        {
          if (registryKey != null)
          {
            object obj = registryKey.GetValue("Content Type");
            if (obj != null)
              str = obj.ToString();
          }
        }
      }
      catch (IOException ex)
      {
      }
      catch (ObjectDisposedException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
      catch (SecurityException ex)
      {
      }
      return str;
    }

    public static string IEUserAgent()
    {
      string str1 = Http.RandomWindowsVersion();
      string str2 = (string) null;
      string str3 = (string) null;
      string str4 = (string) null;
      string str5;
      if (str1.Contains("NT 5.1"))
      {
        str2 = "9.0";
        str3 = "5.0";
        str4 = "5.0";
        str5 = ".NET CLR 2.0.50727; .NET CLR 3.5.30729";
      }
      else if (str1.Contains("NT 6.0"))
      {
        str2 = "9.0";
        str3 = "5.0";
        str4 = "5.0";
        str5 = ".NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.5.30729";
      }
      else
      {
        switch (Http.Rand.Next(3))
        {
          case 0:
            str2 = "10.0";
            str4 = "6.0";
            str3 = "5.0";
            break;
          case 1:
            str2 = "10.6";
            str4 = "6.0";
            str3 = "5.0";
            break;
          case 2:
            str2 = "11.0";
            str4 = "7.0";
            str3 = "5.0";
            break;
        }
        str5 = ".NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E";
      }
      return string.Format("Mozilla/{0} (compatible; MSIE {1}; {2}; Trident/{3}; {4})", (object) str3, (object) str2, (object) str1, (object) str4, (object) str5);
    }

    public static string OperaUserAgent()
    {
      string str1 = (string) null;
      string str2 = (string) null;
      switch (Http.Rand.Next(4))
      {
        case 0:
          str1 = "12.16";
          str2 = "2.12.388";
          break;
        case 1:
          str1 = "12.14";
          str2 = "2.12.388";
          break;
        case 2:
          str1 = "12.02";
          str2 = "2.10.289";
          break;
        case 3:
          str1 = "12.00";
          str2 = "2.10.181";
          break;
      }
      return string.Format("Opera/9.80 ({0}); U) Presto/{1} Version/{2}", (object) Http.RandomWindowsVersion(), (object) str2, (object) str1);
    }

    public static string ChromeUserAgent()
    {
      string str1 = (string) null;
      string str2 = (string) null;
      switch (Http.Rand.Next(5))
      {
        case 0:
          str1 = "41.0.2228.0";
          str2 = "537.36";
          break;
        case 1:
          str1 = "41.0.2227.1";
          str2 = "537.36";
          break;
        case 2:
          str1 = "41.0.2224.3";
          str2 = "537.36";
          break;
        case 3:
          str1 = "41.0.2225.0";
          str2 = "537.36";
          break;
        case 4:
          str1 = "41.0.2226.0";
          str2 = "537.36";
          break;
      }
      return string.Format("Mozilla/5.0 ({0}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{1} Safari/{2}", (object) Http.RandomWindowsVersion(), (object) str1, (object) str2);
    }

    public static string FirefoxUserAgent()
    {
      string str1 = (string) null;
      string str2 = (string) null;
      switch (Http.Rand.Next(5))
      {
        case 0:
          str2 = "36.0";
          str1 = "20100101";
          break;
        case 1:
          str2 = "33.0";
          str1 = "20100101";
          break;
        case 2:
          str2 = "31.0";
          str1 = "20100101";
          break;
        case 3:
          str2 = "29.0";
          str1 = "20120101";
          break;
        case 4:
          str2 = "28.0";
          str1 = "20100101";
          break;
      }
      return string.Format("Mozilla/5.0 ({0}; rv:{1}) Gecko/{2} Firefox/{1}", (object) Http.RandomWindowsVersion(), (object) str2, (object) str1);
    }

    public static string OperaMiniUserAgent()
    {
      string str1 = (string) null;
      string str2 = (string) null;
      string str3 = (string) null;
      string str4 = (string) null;
      switch (Http.Rand.Next(3))
      {
        case 0:
          str1 = "iOS";
          str2 = "7.0.73345";
          str3 = "11.62";
          str4 = "2.10.229";
          break;
        case 1:
          str1 = "J2ME/MIDP";
          str2 = "7.1.23511";
          str3 = "12.00";
          str4 = "2.10.181";
          break;
        case 2:
          str1 = "Android";
          str2 = "7.5.54678";
          str3 = "12.02";
          str4 = "2.10.289";
          break;
      }
      return string.Format("Opera/9.80 ({0}; Opera Mini/{1}/28.2555; U; ru) Presto/{2} Version/{3}", (object) str1, (object) str2, (object) str4, (object) str3);
    }

    private static bool AcceptAllCertifications(
      object sender,
      X509Certificate certification,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
    {
      return true;
    }

    private static bool IsUrlSafeChar(char c)
    {
      if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || (c >= '0' && c <= '9' || c == '!'))
        return true;
      switch (c)
      {
        case '(':
        case ')':
        case '*':
        case '-':
        case '.':
        case '_':
          return true;
        default:
          return false;
      }
    }

    private static char IntToHex(int i)
    {
      if (i <= 9)
        return (char) (i + 48);
      return (char) (i - 10 + 65);
    }

    private static string RandomWindowsVersion()
    {
      string str = "Windows NT ";
      switch (Http.Rand.Next(4))
      {
        case 0:
          str += "5.1";
          break;
        case 1:
          str += "6.0";
          break;
        case 2:
          str += "6.1";
          break;
        case 3:
          str += "6.2";
          break;
      }
      if (Http.Rand.NextDouble() < 0.2)
        str += "; WOW64";
      return str;
    }
  }
}
