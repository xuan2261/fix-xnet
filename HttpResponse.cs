// Decompiled with JetBrains decompiler
// Type: xNet.HttpResponse
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace xNet
{
  [DebuggerDisplay("{ToDebuggerString()}")]
  public sealed class HttpResponse
  {
    private static readonly byte[] _openHtmlSignature = Encoding.ASCII.GetBytes("<html");
    private static readonly byte[] _closeHtmlSignature = Encoding.ASCII.GetBytes("</html>");
    private static readonly Regex _keepAliveTimeoutRegex = new Regex("timeout(|\\s+)=(|\\s+)(?<value>\\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex _keepAliveMaxRegex = new Regex("max(|\\s+)=(|\\s+)(?<value>\\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex _contentCharsetRegex = new Regex("charset(|\\s+)=(|\\s+)(?<value>[a-z,0-9,-]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private readonly Dictionary<string, string> _headers = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private readonly CookieDictionary _rawCookies = new CookieDictionary(false);
    private readonly HttpRequest _request;
    private HttpResponse.ReceiverHelper _receiverHelper;

    public bool HasError { get; private set; }

    public bool MessageBodyLoaded { get; private set; }

    public bool IsOK
    {
      get
      {
        return this.StatusCode == HttpStatusCode.OK;
      }
    }

    public bool HasRedirect
    {
      get
      {
        int statusCode = (int) this.StatusCode;
        return statusCode >= 300 && statusCode < 400 || (this._headers.ContainsKey("Location") || this._headers.ContainsKey("Redirect-Location"));
      }
    }

    public int ReconnectCount { get; internal set; }

    public Uri Address { get; private set; }

    public HttpMethod Method { get; private set; }

    public Version ProtocolVersion { get; private set; }

    public HttpStatusCode StatusCode { get; private set; }

    public Uri RedirectAddress { get; private set; }

    public Encoding CharacterSet { get; private set; }

    public int ContentLength { get; private set; }

    public string ContentType { get; private set; }

    public string Location
    {
      get
      {
        return this[nameof (Location)];
      }
    }

    public CookieDictionary Cookies { get; private set; }

    public int? KeepAliveTimeout { get; private set; }

    public int? MaximumKeepAliveRequests { get; private set; }

    public string this[string headerName]
    {
      get
      {
        if (headerName == null)
          throw new ArgumentNullException(nameof (headerName));
        if (headerName.Length == 0)
          throw ExceptionHelper.EmptyString(nameof (headerName));
        string empty;
        if (!this._headers.TryGetValue(headerName, out empty))
          empty = string.Empty;
        return empty;
      }
    }

    public string this[HttpHeader header]
    {
      get
      {
        return this[Http.Headers[header]];
      }
    }

    internal HttpResponse(HttpRequest request)
    {
      this._request = request;
      this.ContentLength = -1;
      this.ContentType = string.Empty;
    }

    public byte[] ToBytes()
    {
      if (this.HasError)
        throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
      if (this.MessageBodyLoaded)
        return Array.Empty<byte>();
      MemoryStream memoryStream = new MemoryStream(this.ContentLength == -1 ? 0 : this.ContentLength);
      try
      {
        foreach (HttpResponse.BytesWraper bytesWraper in this.GetMessageBodySource())
          memoryStream.Write(bytesWraper.Value, 0, bytesWraper.Length);
      }
      catch (Exception ex)
      {
        this.HasError = true;
        if (ex is IOException || ex is InvalidOperationException)
          throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
        throw;
      }
      if (this.ConnectionClosed())
        this._request.Dispose();
      this.MessageBodyLoaded = true;
      return memoryStream.ToArray();
    }

    public override string ToString()
    {
      if (this.HasError)
        throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
      if (this.MessageBodyLoaded)
        return string.Empty;
      MemoryStream memoryStream = new MemoryStream(this.ContentLength == -1 ? 0 : this.ContentLength);
      try
      {
        foreach (HttpResponse.BytesWraper bytesWraper in this.GetMessageBodySource())
          memoryStream.Write(bytesWraper.Value, 0, bytesWraper.Length);
      }
      catch (Exception ex)
      {
        this.HasError = true;
        if (ex is IOException || ex is InvalidOperationException)
          throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
        throw;
      }
      if (this.ConnectionClosed())
        this._request.Dispose();
      this.MessageBodyLoaded = true;
      return this.CharacterSet.GetString(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
    }

    public void ToFile(string path)
    {
      if (this.HasError)
        throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      if (this.MessageBodyLoaded)
        return;
      try
      {
        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        {
          foreach (HttpResponse.BytesWraper bytesWraper in this.GetMessageBodySource())
            fileStream.Write(bytesWraper.Value, 0, bytesWraper.Length);
        }
      }
      catch (ArgumentException ex)
      {
        throw ExceptionHelper.WrongPath(nameof (path), (Exception) ex);
      }
      catch (NotSupportedException ex)
      {
        throw ExceptionHelper.WrongPath(nameof (path), (Exception) ex);
      }
      catch (Exception ex)
      {
        this.HasError = true;
        if (ex is IOException || ex is InvalidOperationException)
          throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
        throw;
      }
      if (this.ConnectionClosed())
        this._request.Dispose();
      this.MessageBodyLoaded = true;
    }

    public MemoryStream ToMemoryStream()
    {
      if (this.HasError)
        throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
      if (this.MessageBodyLoaded)
        return (MemoryStream) null;
      MemoryStream memoryStream = new MemoryStream(this.ContentLength == -1 ? 0 : this.ContentLength);
      try
      {
        foreach (HttpResponse.BytesWraper bytesWraper in this.GetMessageBodySource())
          memoryStream.Write(bytesWraper.Value, 0, bytesWraper.Length);
      }
      catch (Exception ex)
      {
        this.HasError = true;
        if (ex is IOException || ex is InvalidOperationException)
          throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
        throw;
      }
      if (this.ConnectionClosed())
        this._request.Dispose();
      this.MessageBodyLoaded = true;
      memoryStream.Position = 0L;
      return memoryStream;
    }

    public void None()
    {
      if (this.HasError)
        throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
      if (this.MessageBodyLoaded)
        return;
      if (this.ConnectionClosed())
      {
        this._request.Dispose();
      }
      else
      {
        try
        {
          foreach (HttpResponse.BytesWraper bytesWraper in this.GetMessageBodySource())
            ;
        }
        catch (Exception ex)
        {
          this.HasError = true;
          if (ex is IOException || ex is InvalidOperationException)
            throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
          throw;
        }
      }
      this.MessageBodyLoaded = true;
    }

    public bool ContainsCookie(string name)
    {
      if (this.Cookies == null)
        return false;
      return this.Cookies.ContainsKey(name);
    }

    public bool ContainsRawCookie(string name)
    {
      return this._rawCookies.ContainsKey(name);
    }

    public string GetRawCookie(string name)
    {
      string empty;
      if (!this._rawCookies.TryGetValue(name, out empty))
        empty = string.Empty;
      return empty;
    }

    public Dictionary<string, string>.Enumerator EnumerateRawCookies()
    {
      return this._rawCookies.GetEnumerator();
    }

    public bool ContainsHeader(string headerName)
    {
      if (headerName == null)
        throw new ArgumentNullException(nameof (headerName));
      if (headerName.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (headerName));
      return this._headers.ContainsKey(headerName);
    }

    public bool ContainsHeader(HttpHeader header)
    {
      return this.ContainsHeader(Http.Headers[header]);
    }

    public Dictionary<string, string>.Enumerator EnumerateHeaders()
    {
      return this._headers.GetEnumerator();
    }

    internal long LoadResponse(HttpMethod method)
    {
      this.Method = method;
      this.Address = this._request.Address;
      this.HasError = false;
      this.MessageBodyLoaded = false;
      this.KeepAliveTimeout = new int?();
      this.MaximumKeepAliveRequests = new int?();
      this._headers.Clear();
      this._rawCookies.Clear();
      this.Cookies = this._request.Cookies == null || this._request.Cookies.IsLocked ? new CookieDictionary(false) : this._request.Cookies;
      if (this._receiverHelper == null)
        this._receiverHelper = new HttpResponse.ReceiverHelper(this._request.TcpClient.ReceiveBufferSize);
      this._receiverHelper.Init(this._request.ClientStream);
      try
      {
        this.ReceiveStartingLine();
        this.ReceiveHeaders();
        this.RedirectAddress = this.GetLocation();
        this.CharacterSet = this.GetCharacterSet();
        this.ContentLength = this.GetContentLength();
        this.ContentType = this.GetContentType();
        this.KeepAliveTimeout = this.GetKeepAliveTimeout();
        this.MaximumKeepAliveRequests = this.GetKeepAliveMax();
      }
      catch (Exception ex)
      {
        this.HasError = true;
        if (ex is IOException)
          throw this.NewHttpException(Resources.HttpException_FailedReceiveResponse, ex);
        throw;
      }
      if (this.ContentLength == 0 || this.Method == HttpMethod.HEAD || (this.StatusCode == HttpStatusCode.Continue || this.StatusCode == HttpStatusCode.NoContent) || this.StatusCode == HttpStatusCode.NotModified)
        this.MessageBodyLoaded = true;
      long position = (long) this._receiverHelper.Position;
      if (this.ContentLength > 0)
        position += (long) this.ContentLength;
      return position;
    }

    private void ReceiveStartingLine()
    {
      string str1;
      do
      {
        str1 = this._receiverHelper.ReadLine();
        if (str1.Length == 0)
        {
          HttpException httpException = this.NewHttpException(Resources.HttpException_ReceivedEmptyResponse, (Exception) null);
          httpException.EmptyMessageBody = true;
          throw httpException;
        }
      }
      while (str1 == "\r\n");
      string input = str1.Substring("HTTP/", " ", StringComparison.Ordinal);
      string str2 = str1.Substring(" ", " ", StringComparison.Ordinal);
      if (str2.Length == 0)
        str2 = str1.Substring(" ", "\r\n", StringComparison.Ordinal);
      if (input.Length == 0 || str2.Length == 0)
        throw this.NewHttpException(Resources.HttpException_ReceivedEmptyResponse, (Exception) null);
      this.ProtocolVersion = Version.Parse(input);
      this.StatusCode = (HttpStatusCode) Enum.Parse(typeof (HttpStatusCode), str2);
    }

    private void SetCookie(string value)
    {
      if (value.Length == 0)
        return;
      int num1 = value.IndexOf(';');
      int length = value.IndexOf('=');
      if (length == -1)
        throw this.NewHttpException(string.Format(Resources.HttpException_WrongCookie, (object) value, (object) this.Address.Host), (Exception) null);
      string key = value.Substring(0, length);
      string str;
      if (num1 == -1)
      {
        str = value.Substring(length + 1);
      }
      else
      {
        str = value.Substring(length + 1, num1 - length - 1);
        int startIndex1 = value.IndexOf("expires=");
        if (startIndex1 != -1)
        {
          int num2 = value.IndexOf(';', startIndex1);
          int startIndex2 = startIndex1 + 8;
          DateTime result;
          if (DateTime.TryParse(num2 != -1 ? value.Substring(startIndex2, num2 - startIndex2) : value.Substring(startIndex2), out result) && result < DateTime.Now)
            this.Cookies.Remove(key);
        }
      }
      if (str.Length == 0 || str.Equals("deleted", StringComparison.OrdinalIgnoreCase))
        this.Cookies.Remove(key);
      else
        this.Cookies[key] = str;
      this._rawCookies[key] = value;
    }

    private void ReceiveHeaders()
    {
      string str1;
      while (true)
      {
        str1 = this._receiverHelper.ReadLine();
        if (!(str1 == "\r\n"))
        {
          int length = str1.IndexOf(':');
          if (length != -1)
          {
            string index = str1.Substring(0, length);
            string str2 = str1.Substring(length + 1).Trim(' ', '\t', '\r', '\n');
            if (index.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
              this.SetCookie(str2);
            else
              this._headers[index] = str2;
          }
          else
            goto label_2;
        }
        else
          break;
      }
      return;
label_2:
      throw this.NewHttpException(string.Format(Resources.HttpException_WrongHeader, (object) str1, (object) this.Address.Host), (Exception) null);
    }

    private IEnumerable<HttpResponse.BytesWraper> GetMessageBodySource()
    {
      if (this._headers.ContainsKey("Content-Encoding"))
        return this.GetMessageBodySourceZip();
      return this.GetMessageBodySourceStd();
    }

    private IEnumerable<HttpResponse.BytesWraper> GetMessageBodySourceStd()
    {
      if (this._headers.ContainsKey("Transfer-Encoding"))
        return this.ReceiveMessageBodyChunked();
      if (this.ContentLength != -1)
        return this.ReceiveMessageBody(this.ContentLength);
      return this.ReceiveMessageBody(this._request.ClientStream);
    }

    private IEnumerable<HttpResponse.BytesWraper> GetMessageBodySourceZip()
    {
      if (this._headers.ContainsKey("Transfer-Encoding"))
        return this.ReceiveMessageBodyChunkedZip();
      if (this.ContentLength != -1)
        return this.ReceiveMessageBodyZip(this.ContentLength);
      return this.ReceiveMessageBody(this.GetZipStream((Stream) new HttpResponse.ZipWraperStream(this._request.ClientStream, this._receiverHelper)));
    }

    private IEnumerable<HttpResponse.BytesWraper> ReceiveMessageBody(
      Stream stream)
    {
      HttpResponse.BytesWraper bytesWraper = new HttpResponse.BytesWraper();
      int bufferSize = this._request.TcpClient.ReceiveBufferSize;
      byte[] buffer = new byte[bufferSize];
      bytesWraper.Value = buffer;
      int begBytesRead = 0;
      if (stream is GZipStream || stream is DeflateStream)
      {
        begBytesRead = stream.Read(buffer, 0, bufferSize);
      }
      else
      {
        if (this._receiverHelper.HasData)
          begBytesRead = this._receiverHelper.Read(buffer, 0, bufferSize);
        if (begBytesRead < bufferSize)
          begBytesRead += stream.Read(buffer, begBytesRead, bufferSize - begBytesRead);
      }
      bytesWraper.Length = begBytesRead;
      yield return bytesWraper;
      bool isHtml = this.FindSignature(buffer, begBytesRead, HttpResponse._openHtmlSignature);
      if (!isHtml || !this.FindSignature(buffer, begBytesRead, HttpResponse._closeHtmlSignature))
      {
        int sourceLength;
        while (true)
        {
          sourceLength = stream.Read(buffer, 0, bufferSize);
          if (isHtml)
          {
            if (sourceLength == 0)
            {
              this.WaitData();
              continue;
            }
            if (this.FindSignature(buffer, sourceLength, HttpResponse._closeHtmlSignature))
              break;
          }
          else if (sourceLength == 0)
            goto label_8;
          bytesWraper.Length = sourceLength;
          yield return bytesWraper;
        }
        bytesWraper.Length = sourceLength;
        yield return bytesWraper;
        yield break;
label_8:;
      }
    }

    private IEnumerable<HttpResponse.BytesWraper> ReceiveMessageBody(
      int contentLength)
    {
      Stream stream = this._request.ClientStream;
      HttpResponse.BytesWraper bytesWraper = new HttpResponse.BytesWraper();
      int bufferSize = this._request.TcpClient.ReceiveBufferSize;
      byte[] buffer = new byte[bufferSize];
      bytesWraper.Value = buffer;
      int totalBytesRead = 0;
      while (totalBytesRead != contentLength)
      {
        int num = !this._receiverHelper.HasData ? stream.Read(buffer, 0, bufferSize) : this._receiverHelper.Read(buffer, 0, bufferSize);
        if (num == 0)
        {
          this.WaitData();
        }
        else
        {
          totalBytesRead += num;
          bytesWraper.Length = num;
          yield return bytesWraper;
        }
      }
    }

    private IEnumerable<HttpResponse.BytesWraper> ReceiveMessageBodyChunked()
    {
      Stream stream = this._request.ClientStream;
      HttpResponse.BytesWraper bytesWraper = new HttpResponse.BytesWraper();
      int bufferSize = this._request.TcpClient.ReceiveBufferSize;
      byte[] buffer = new byte[bufferSize];
      bytesWraper.Value = buffer;
label_1:
      string str1;
      do
      {
        str1 = this._receiverHelper.ReadLine();
      }
      while (str1 == "\r\n");
      string str2 = str1.Trim(' ', '\r', '\n');
      if (!(str2 == string.Empty))
      {
        int totalBytesRead = 0;
        int blockLength;
        try
        {
          blockLength = Convert.ToInt32(str2, 16);
        }
        catch (Exception ex)
        {
          if (ex is FormatException || ex is OverflowException)
            throw this.NewHttpException(string.Format(Resources.HttpException_WrongChunkedBlockLength, (object) str2), ex);
          throw;
        }
        if (blockLength != 0)
        {
          while (totalBytesRead != blockLength)
          {
            int num1 = blockLength - totalBytesRead;
            if (num1 > bufferSize)
              num1 = bufferSize;
            int num2 = !this._receiverHelper.HasData ? stream.Read(buffer, 0, num1) : this._receiverHelper.Read(buffer, 0, num1);
            if (num2 == 0)
            {
              this.WaitData();
            }
            else
            {
              totalBytesRead += num2;
              bytesWraper.Length = num2;
              yield return bytesWraper;
            }
          }
          goto label_1;
        }
      }
    }

    private IEnumerable<HttpResponse.BytesWraper> ReceiveMessageBodyZip(
      int contentLength)
    {
      HttpResponse.BytesWraper bytesWraper = new HttpResponse.BytesWraper();
      HttpResponse.ZipWraperStream streamWrapper = new HttpResponse.ZipWraperStream(this._request.ClientStream, this._receiverHelper);
      using (Stream stream = this.GetZipStream((Stream) streamWrapper))
      {
        int bufferSize = this._request.TcpClient.ReceiveBufferSize;
        byte[] buffer = new byte[bufferSize];
        bytesWraper.Value = buffer;
        while (true)
        {
          int num = stream.Read(buffer, 0, bufferSize);
          if (num == 0)
          {
            if (streamWrapper.TotalBytesRead != contentLength)
              this.WaitData();
            else
              break;
          }
          else
          {
            bytesWraper.Length = num;
            yield return bytesWraper;
          }
        }
      }
    }

    private IEnumerable<HttpResponse.BytesWraper> ReceiveMessageBodyChunkedZip()
    {
      HttpResponse.BytesWraper bytesWraper = new HttpResponse.BytesWraper();
      HttpResponse.ZipWraperStream streamWrapper = new HttpResponse.ZipWraperStream(this._request.ClientStream, this._receiverHelper);
      bool flag;
      using (Stream stream = this.GetZipStream((Stream) streamWrapper))
      {
        int bufferSize = this._request.TcpClient.ReceiveBufferSize;
        byte[] buffer = new byte[bufferSize];
        bytesWraper.Value = buffer;
label_1:
        string str1;
        do
        {
          str1 = this._receiverHelper.ReadLine();
        }
        while (str1 == "\r\n");
        string str2 = str1.Trim(' ', '\r', '\n');
        if (str2 == string.Empty)
        {
          flag = false;
        }
        else
        {
          int blockLength;
          try
          {
            blockLength = Convert.ToInt32(str2, 16);
          }
          catch (Exception ex)
          {
            if (ex is FormatException || ex is OverflowException)
              throw this.NewHttpException(string.Format(Resources.HttpException_WrongChunkedBlockLength, (object) str2), ex);
            throw;
          }
          if (blockLength == 0)
          {
            flag = false;
          }
          else
          {
            streamWrapper.TotalBytesRead = 0;
            streamWrapper.LimitBytesRead = blockLength;
            while (true)
            {
              int num = stream.Read(buffer, 0, bufferSize);
              if (num == 0)
              {
                if (streamWrapper.TotalBytesRead != blockLength)
                  this.WaitData();
                else
                  goto label_1;
              }
              else
              {
                bytesWraper.Length = num;
                yield return bytesWraper;
              }
            }
          }
        }
      }
      return flag;
    }

    private bool ConnectionClosed()
    {
      return this._headers.ContainsKey("Connection") && this._headers["Connection"].Equals("close", StringComparison.OrdinalIgnoreCase) || this._headers.ContainsKey("Proxy-Connection") && this._headers["Proxy-Connection"].Equals("close", StringComparison.OrdinalIgnoreCase);
    }

    private int? GetKeepAliveTimeout()
    {
      if (!this._headers.ContainsKey("Keep-Alive"))
        return new int?();
      string header = this._headers["Keep-Alive"];
      System.Text.RegularExpressions.Match match = HttpResponse._keepAliveTimeoutRegex.Match(header);
      if (match.Success)
        return new int?(int.Parse(match.Groups["value"].Value) * 1000);
      return new int?();
    }

    private int? GetKeepAliveMax()
    {
      if (!this._headers.ContainsKey("Keep-Alive"))
        return new int?();
      string header = this._headers["Keep-Alive"];
      System.Text.RegularExpressions.Match match = HttpResponse._keepAliveMaxRegex.Match(header);
      if (match.Success)
        return new int?(int.Parse(match.Groups["value"].Value));
      return new int?();
    }

    private Uri GetLocation()
    {
      string relativeUri;
      if (!this._headers.TryGetValue("Location", out relativeUri))
        this._headers.TryGetValue("Redirect-Location", out relativeUri);
      if (string.IsNullOrEmpty(relativeUri))
        return (Uri) null;
      Uri result;
      Uri.TryCreate(this._request.Address, relativeUri, out result);
      return result;
    }

    private Encoding GetCharacterSet()
    {
      if (!this._headers.ContainsKey("Content-Type"))
        return this._request.CharacterSet ?? Encoding.Default;
      string header = this._headers["Content-Type"];
      System.Text.RegularExpressions.Match match = HttpResponse._contentCharsetRegex.Match(header);
      if (!match.Success)
        return this._request.CharacterSet ?? Encoding.Default;
      Group group = match.Groups["value"];
      try
      {
        return Encoding.GetEncoding(group.Value);
      }
      catch (ArgumentException ex)
      {
        return this._request.CharacterSet ?? Encoding.Default;
      }
    }

    private int GetContentLength()
    {
      if (!this._headers.ContainsKey("Content-Length"))
        return -1;
      int result;
      int.TryParse(this._headers["Content-Length"], out result);
      return result;
    }

    private string GetContentType()
    {
      if (!this._headers.ContainsKey("Content-Type"))
        return string.Empty;
      string str = this._headers["Content-Type"];
      int length = str.IndexOf(';');
      if (length != -1)
        str = str.Substring(0, length);
      return str;
    }

    private void WaitData()
    {
      int num1 = 0;
      int num2 = this._request.TcpClient.ReceiveTimeout < 10 ? 10 : this._request.TcpClient.ReceiveTimeout;
      while (!this._request.ClientNetworkStream.DataAvailable)
      {
        if (num1 >= num2)
          throw this.NewHttpException(Resources.HttpException_WaitDataTimeout, (Exception) null);
        num1 += 10;
        Thread.Sleep(10);
      }
    }

    private Stream GetZipStream(Stream stream)
    {
      string lower = this._headers["Content-Encoding"].ToLower();
      if (lower != null)
      {
        if (lower == "gzip")
          return (Stream) new GZipStream(stream, CompressionMode.Decompress, true);
        if (lower == "deflate")
          return (Stream) new DeflateStream(stream, CompressionMode.Decompress, true);
      }
      throw new InvalidOperationException(string.Format(Resources.InvalidOperationException_NotSupportedEncodingFormat, (object) lower));
    }

    private bool FindSignature(byte[] source, int sourceLength, byte[] signature)
    {
      int num = sourceLength - signature.Length + 1;
      for (int index1 = 0; index1 < num; ++index1)
      {
        for (int index2 = 0; index2 < signature.Length; ++index2)
        {
          char lower = (char) source[index2 + index1];
          if (char.IsLetter(lower))
            lower = char.ToLower(lower);
          if ((int) (byte) lower == (int) signature[index2])
          {
            if (index2 == signature.Length - 1)
              return true;
          }
          else
            break;
        }
      }
      return false;
    }

    private HttpException NewHttpException(string message, Exception innerException = null)
    {
      return new HttpException(string.Format(message, (object) this.Address.Host), HttpExceptionStatus.ReceiveFailure, HttpStatusCode.None, innerException);
    }

    private string ToDebuggerString()
    {
      return string.Format("Status Code: {0}; Reason Phrase: '{1}'; Content Type: '{2}'", (object) (int) this.StatusCode, (object) this.StatusCode.ToString(), (object) this.ContentType);
    }

    private sealed class BytesWraper
    {
      public int Length { get; set; }

      public byte[] Value { get; set; }
    }

    private sealed class ReceiverHelper
    {
      private byte[] _lineBuffer = new byte[1000];
      private const int InitialLineSize = 1000;
      private Stream _stream;
      private readonly byte[] _buffer;
      private readonly int _bufferSize;
      private int _linePosition;

      public bool HasData
      {
        get
        {
          return (uint) (this.Length - this.Position) > 0U;
        }
      }

      public int Length { get; private set; }

      public int Position { get; private set; }

      public ReceiverHelper(int bufferSize)
      {
        this._bufferSize = bufferSize;
        this._buffer = new byte[this._bufferSize];
      }

      public void Init(Stream stream)
      {
        this._stream = stream;
        this._linePosition = 0;
        this.Length = 0;
        this.Position = 0;
      }

      public string ReadLine()
      {
        this._linePosition = 0;
        while (true)
        {
          do
          {
            if (this.Position == this.Length)
            {
              this.Position = 0;
              this.Length = this._stream.Read(this._buffer, 0, this._bufferSize);
              if (this.Length == 0)
                goto label_6;
            }
            byte num = this._buffer[this.Position++];
            this._lineBuffer[this._linePosition++] = num;
            if (num == (byte) 10)
              goto label_6;
          }
          while (this._linePosition != this._lineBuffer.Length);
          byte[] numArray = new byte[this._lineBuffer.Length * 2];
          this._lineBuffer.CopyTo((Array) numArray, 0);
          this._lineBuffer = numArray;
        }
label_6:
        return Encoding.ASCII.GetString(this._lineBuffer, 0, this._linePosition);
      }

      public int Read(byte[] buffer, int index, int length)
      {
        int length1 = this.Length - this.Position;
        if (length1 > length)
          length1 = length;
        Array.Copy((Array) this._buffer, this.Position, (Array) buffer, index, length1);
        this.Position += length1;
        return length1;
      }
    }

    private sealed class ZipWraperStream : Stream
    {
      private readonly Stream _baseStream;
      private readonly HttpResponse.ReceiverHelper _receiverHelper;

      public int BytesRead { get; private set; }

      public int TotalBytesRead { get; set; }

      public int LimitBytesRead { get; set; }

      public override bool CanRead
      {
        get
        {
          return this._baseStream.CanRead;
        }
      }

      public override bool CanSeek
      {
        get
        {
          return this._baseStream.CanSeek;
        }
      }

      public override bool CanTimeout
      {
        get
        {
          return this._baseStream.CanTimeout;
        }
      }

      public override bool CanWrite
      {
        get
        {
          return this._baseStream.CanWrite;
        }
      }

      public override long Length
      {
        get
        {
          return this._baseStream.Length;
        }
      }

      public override long Position
      {
        get
        {
          return this._baseStream.Position;
        }
        set
        {
          this._baseStream.Position = value;
        }
      }

      public ZipWraperStream(Stream baseStream, HttpResponse.ReceiverHelper receiverHelper)
      {
        this._baseStream = baseStream;
        this._receiverHelper = receiverHelper;
      }

      public override void Flush()
      {
        this._baseStream.Flush();
      }

      public override void SetLength(long value)
      {
        this._baseStream.SetLength(value);
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        return this._baseStream.Seek(offset, origin);
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        if (this.LimitBytesRead != 0)
        {
          int num = this.LimitBytesRead - this.TotalBytesRead;
          if (num == 0)
            return 0;
          if (num > buffer.Length)
            num = buffer.Length;
          this.BytesRead = !this._receiverHelper.HasData ? this._baseStream.Read(buffer, offset, num) : this._receiverHelper.Read(buffer, offset, num);
        }
        else
          this.BytesRead = !this._receiverHelper.HasData ? this._baseStream.Read(buffer, offset, count) : this._receiverHelper.Read(buffer, offset, count);
        this.TotalBytesRead += this.BytesRead;
        return this.BytesRead;
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        this._baseStream.Write(buffer, offset, count);
      }
    }
  }
}
