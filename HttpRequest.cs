// Decompiled with JetBrains decompiler
// Type: xNet.HttpRequest
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security;
using System.Security.Authentication;
using System.Text;
using System.Threading;

namespace xNet
{
  public class HttpRequest : IDisposable
  {
    public static readonly Version ProtocolVersion = new Version(1, 1);
    private static readonly List<string> closedHeaders = new List<string>()
    {
      "Accept-Encoding",
      "Content-Length",
      "Content-Type",
      "Connection",
      "Proxy-Connection",
      "Host"
    };
    private int maximumAutomaticRedirections = 5;
    private int connectTimeout = 60000;
    private int readWriteTimeout = 60000;
    private int keepAliveTimeout = 30000;
    private int maximumKeepAliveRequests = 100;
    private int reconnectLimit = 3;
    private int reconnectDelay = 100;
    private readonly Dictionary<string, string> permanentHeaders = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private HttpResponse response;
    private TcpClient connection;
    private Stream connectionCommonStream;
    private NetworkStream connectionNetworkStream;
    private ProxyClient currentProxy;
    private int redirectionCount;
    private DateTime whenConnectionIdle;
    private int keepAliveRequestCount;
    private bool keepAliveReconnected;
    private bool canReportBytesReceived;
    private int reconnectCount;
    private HttpMethod _method;
    private HttpContent content;
    private RequestParams temporaryParams;
    private RequestParams temporaryUrlParams;
    private Dictionary<string, string> temporaryHeaders;
    private MultipartContent temporaryMultipartContent;
    private long bytesSent;
    private long totalBytesSent;
    private long bytesReceived;
    private long totalBytesReceived;
    private EventHandler<UploadProgressChangedEventArgs> uploadProgressChangedHandler;
    private EventHandler<DownloadProgressChangedEventArgs> downloadProgressChangedHandler;
    public RemoteCertificateValidationCallback SslCertificateValidatorCallback;

    public static bool UseIeProxy { get; set; }

    public static bool DisableProxyForLocalAddress { get; set; }

    public static ProxyClient GlobalProxy { get; set; }

    public event EventHandler<UploadProgressChangedEventArgs> UploadProgressChanged
    {
      add
      {
        this.uploadProgressChangedHandler += value;
      }
      remove
      {
        this.uploadProgressChangedHandler -= value;
      }
    }

    public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged
    {
      add
      {
        this.downloadProgressChangedHandler += value;
      }
      remove
      {
        this.downloadProgressChangedHandler -= value;
      }
    }

    public Uri BaseAddress { get; set; }

    public Uri Address { get; private set; }

    public HttpResponse Response
    {
      get
      {
        return this.response;
      }
    }

    public ProxyClient Proxy { get; set; }

    public bool AllowAutoRedirect { get; set; }

    public int MaximumAutomaticRedirections
    {
      get
      {
        return this.maximumAutomaticRedirections;
      }
      set
      {
        if (value < 1)
          throw ExceptionHelper.CanNotBeLess<int>(nameof (MaximumAutomaticRedirections), 1);
        this.maximumAutomaticRedirections = value;
      }
    }

    public int ConnectTimeout
    {
      get
      {
        return this.connectTimeout;
      }
      set
      {
        if (value < 0)
          throw ExceptionHelper.CanNotBeLess<int>(nameof (ConnectTimeout), 0);
        this.connectTimeout = value;
      }
    }

    public int ReadWriteTimeout
    {
      get
      {
        return this.readWriteTimeout;
      }
      set
      {
        if (value < 0)
          throw ExceptionHelper.CanNotBeLess<int>(nameof (ReadWriteTimeout), 0);
        this.readWriteTimeout = value;
      }
    }

    public bool IgnoreProtocolErrors { get; set; }

    public bool KeepAlive { get; set; }

    public int KeepAliveTimeout
    {
      get
      {
        return this.keepAliveTimeout;
      }
      set
      {
        if (value < 0)
          throw ExceptionHelper.CanNotBeLess<int>(nameof (KeepAliveTimeout), 0);
        this.keepAliveTimeout = value;
      }
    }

    public int MaximumKeepAliveRequests
    {
      get
      {
        return this.maximumKeepAliveRequests;
      }
      set
      {
        if (value < 1)
          throw ExceptionHelper.CanNotBeLess<int>(nameof (MaximumKeepAliveRequests), 1);
        this.maximumKeepAliveRequests = value;
      }
    }

    public bool Reconnect { get; set; }

    public int ReconnectLimit
    {
      get
      {
        return this.reconnectLimit;
      }
      set
      {
        if (value < 1)
          throw ExceptionHelper.CanNotBeLess<int>(nameof (ReconnectLimit), 1);
        this.reconnectLimit = value;
      }
    }

    public int ReconnectDelay
    {
      get
      {
        return this.reconnectDelay;
      }
      set
      {
        if (value < 0)
          throw ExceptionHelper.CanNotBeLess<int>(nameof (ReconnectDelay), 0);
        this.reconnectDelay = value;
      }
    }

    public CultureInfo Culture { get; set; }

    public Encoding CharacterSet { get; set; }

    public bool EnableEncodingContent { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string UserAgent
    {
      get
      {
        return this["User-Agent"];
      }
      set
      {
        this["User-Agent"] = value;
      }
    }

    public string Referer
    {
      get
      {
        return this[nameof (Referer)];
      }
      set
      {
        this[nameof (Referer)] = value;
      }
    }

    public string Authorization
    {
      get
      {
        return this[nameof (Authorization)];
      }
      set
      {
        this[nameof (Authorization)] = value;
      }
    }

    public CookieDictionary Cookies { get; set; }

    internal TcpClient TcpClient
    {
      get
      {
        return this.connection;
      }
    }

    internal Stream ClientStream
    {
      get
      {
        return this.connectionCommonStream;
      }
    }

    internal NetworkStream ClientNetworkStream
    {
      get
      {
        return this.connectionNetworkStream;
      }
    }

    private MultipartContent AddedMultipartData
    {
      get
      {
        if (this.temporaryMultipartContent == null)
          this.temporaryMultipartContent = new MultipartContent();
        return this.temporaryMultipartContent;
      }
    }

    public string this[string headerName]
    {
      get
      {
        if (headerName == null)
          throw new ArgumentNullException(nameof (headerName));
        if (headerName.Length == 0)
          throw ExceptionHelper.EmptyString(nameof (headerName));
        string empty;
        if (!this.permanentHeaders.TryGetValue(headerName, out empty))
          empty = string.Empty;
        return empty;
      }
      set
      {
        if (headerName == null)
          throw new ArgumentNullException(nameof (headerName));
        if (headerName.Length == 0)
          throw ExceptionHelper.EmptyString(nameof (headerName));
        if (this.IsClosedHeader(headerName))
          throw new ArgumentException(string.Format(Resources.ArgumentException_HttpRequest_SetNotAvailableHeader, (object) headerName), nameof (headerName));
        if (string.IsNullOrEmpty(value))
          this.permanentHeaders.Remove(headerName);
        else
          this.permanentHeaders[headerName] = value;
      }
    }

    public string this[HttpHeader header]
    {
      get
      {
        return this[Http.Headers[header]];
      }
      set
      {
        this[Http.Headers[header]] = value;
      }
    }

    public HttpRequest()
    {
      this.Init();
    }

    public HttpRequest(string baseAddress)
    {
      if (baseAddress == null)
        throw new ArgumentNullException(nameof (baseAddress));
      if (baseAddress.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (baseAddress));
      if (!baseAddress.StartsWith("http"))
        baseAddress = "http://" + baseAddress;
      Uri uri = new Uri(baseAddress);
      if (!uri.IsAbsoluteUri)
        throw new ArgumentException(Resources.ArgumentException_OnlyAbsoluteUri, nameof (baseAddress));
      this.BaseAddress = uri;
      this.Init();
    }

    public HttpRequest(Uri baseAddress)
    {
      if (baseAddress == (Uri) null)
        throw new ArgumentNullException(nameof (baseAddress));
      if (!baseAddress.IsAbsoluteUri)
        throw new ArgumentException(Resources.ArgumentException_OnlyAbsoluteUri, nameof (baseAddress));
      this.BaseAddress = baseAddress;
      this.Init();
    }

    public HttpResponse Get(string address, RequestParams urlParams = null)
    {
      if (urlParams != null)
        this.temporaryUrlParams = urlParams;
      return this.Raw(HttpMethod.GET, address, (HttpContent) null);
    }

    public HttpResponse Get(Uri address, RequestParams urlParams = null)
    {
      if (urlParams != null)
        this.temporaryUrlParams = urlParams;
      return this.Raw(HttpMethod.GET, address, (HttpContent) null);
    }

    public HttpResponse Post(string address)
    {
      return this.Raw(HttpMethod.POST, address, (HttpContent) null);
    }

    public HttpResponse Post(Uri address)
    {
      return this.Raw(HttpMethod.POST, address, (HttpContent) null);
    }

    public HttpResponse Post(string address, RequestParams reqParams, bool dontEscape = false)
    {
      if (reqParams == null)
        throw new ArgumentNullException(nameof (reqParams));
      return this.Raw(HttpMethod.POST, address, (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) reqParams, dontEscape, this.CharacterSet));
    }

    public HttpResponse Post(Uri address, RequestParams reqParams, bool dontEscape = false)
    {
      if (reqParams == null)
        throw new ArgumentNullException(nameof (reqParams));
      return this.Raw(HttpMethod.POST, address, (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) reqParams, dontEscape, this.CharacterSet));
    }

    public HttpResponse Post(string address, string str, string contentType)
    {
      if (str == null)
        throw new ArgumentNullException(nameof (str));
      if (str.Length == 0)
        throw new ArgumentNullException(nameof (str));
      if (contentType == null)
        throw new ArgumentNullException(nameof (contentType));
      if (contentType.Length == 0)
        throw new ArgumentNullException(nameof (contentType));
      StringContent stringContent1 = new StringContent(str);
      stringContent1.ContentType = contentType;
      StringContent stringContent2 = stringContent1;
      return this.Raw(HttpMethod.POST, address, (HttpContent) stringContent2);
    }

    public HttpResponse Post(Uri address, string str, string contentType)
    {
      if (str == null)
        throw new ArgumentNullException(nameof (str));
      if (str.Length == 0)
        throw new ArgumentNullException(nameof (str));
      if (contentType == null)
        throw new ArgumentNullException(nameof (contentType));
      if (contentType.Length == 0)
        throw new ArgumentNullException(nameof (contentType));
      StringContent stringContent1 = new StringContent(str);
      stringContent1.ContentType = contentType;
      StringContent stringContent2 = stringContent1;
      return this.Raw(HttpMethod.POST, address, (HttpContent) stringContent2);
    }

    public HttpResponse Post(string address, byte[] bytes, string contentType = "application/octet-stream")
    {
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      if (contentType == null)
        throw new ArgumentNullException(nameof (contentType));
      if (contentType.Length == 0)
        throw new ArgumentNullException(nameof (contentType));
      BytesContent bytesContent1 = new BytesContent(bytes);
      bytesContent1.ContentType = contentType;
      BytesContent bytesContent2 = bytesContent1;
      return this.Raw(HttpMethod.POST, address, (HttpContent) bytesContent2);
    }

    public HttpResponse Post(Uri address, byte[] bytes, string contentType = "application/octet-stream")
    {
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      if (contentType == null)
        throw new ArgumentNullException(nameof (contentType));
      if (contentType.Length == 0)
        throw new ArgumentNullException(nameof (contentType));
      BytesContent bytesContent1 = new BytesContent(bytes);
      bytesContent1.ContentType = contentType;
      BytesContent bytesContent2 = bytesContent1;
      return this.Raw(HttpMethod.POST, address, (HttpContent) bytesContent2);
    }

    public HttpResponse Post(string address, Stream stream, string contentType = "application/octet-stream")
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      if (contentType == null)
        throw new ArgumentNullException(nameof (contentType));
      if (contentType.Length == 0)
        throw new ArgumentNullException(nameof (contentType));
      StreamContent streamContent1 = new StreamContent(stream, 32768);
      streamContent1.ContentType = contentType;
      StreamContent streamContent2 = streamContent1;
      return this.Raw(HttpMethod.POST, address, (HttpContent) streamContent2);
    }

    public HttpResponse Post(Uri address, Stream stream, string contentType = "application/octet-stream")
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      if (contentType == null)
        throw new ArgumentNullException(nameof (contentType));
      if (contentType.Length == 0)
        throw new ArgumentNullException(nameof (contentType));
      StreamContent streamContent1 = new StreamContent(stream, 32768);
      streamContent1.ContentType = contentType;
      StreamContent streamContent2 = streamContent1;
      return this.Raw(HttpMethod.POST, address, (HttpContent) streamContent2);
    }

    public HttpResponse Post(string address, string path)
    {
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      if (path.Length == 0)
        throw new ArgumentNullException(nameof (path));
      return this.Raw(HttpMethod.POST, address, (HttpContent) new FileContent(path, 32768));
    }

    public HttpResponse Post(Uri address, string path)
    {
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      if (path.Length == 0)
        throw new ArgumentNullException(nameof (path));
      return this.Raw(HttpMethod.POST, address, (HttpContent) new FileContent(path, 32768));
    }

    public HttpResponse Post(string address, HttpContent content)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      return this.Raw(HttpMethod.POST, address, content);
    }

    public HttpResponse Post(Uri address, HttpContent content)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      return this.Raw(HttpMethod.POST, address, content);
    }

    public HttpResponse Raw(HttpMethod method, string address, HttpContent content = null)
    {
      if (address == null)
        throw new ArgumentNullException(nameof (address));
      if (address.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (address));
      Uri address1 = new Uri(address, UriKind.RelativeOrAbsolute);
      return this.Raw(method, address1, content);
    }

    public HttpResponse Raw(HttpMethod method, Uri address, HttpContent content = null)
    {
      if (address == (Uri) null)
        throw new ArgumentNullException(nameof (address));
      if (!address.IsAbsoluteUri)
        address = this.GetRequestAddress(this.BaseAddress, address);
      if (this.temporaryUrlParams != null)
        address = new UriBuilder(address)
        {
          Query = Http.ToQueryString((IEnumerable<KeyValuePair<string, string>>) this.temporaryUrlParams, true)
        }.Uri;
      if (content == null)
      {
        if (this.temporaryParams != null)
          content = (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) this.temporaryParams, false, this.CharacterSet);
        else if (this.temporaryMultipartContent != null)
          content = (HttpContent) this.temporaryMultipartContent;
      }
      try
      {
        return this.Request(method, address, content);
      }
      finally
      {
        content?.Dispose();
        this.ClearRequestData();
      }
    }

    public HttpRequest AddUrlParam(string name, object value = null)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (name));
      if (this.temporaryUrlParams == null)
        this.temporaryUrlParams = new RequestParams();
      this.temporaryUrlParams[name] = value;
      return this;
    }

    public HttpRequest AddParam(string name, object value = null)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (name));
      if (this.temporaryParams == null)
        this.temporaryParams = new RequestParams();
      this.temporaryParams[name] = value;
      return this;
    }

    public HttpRequest AddField(string name, object value = null)
    {
      return this.AddField(name, value, this.CharacterSet ?? Encoding.UTF8);
    }

    public HttpRequest AddField(string name, object value, Encoding encoding)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (name));
      if (encoding == null)
        throw new ArgumentNullException(nameof (encoding));
      this.AddedMultipartData.Add((HttpContent) new StringContent(value == null ? string.Empty : value.ToString(), encoding), name);
      return this;
    }

    public HttpRequest AddField(string name, byte[] value)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (name));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.AddedMultipartData.Add((HttpContent) new BytesContent(value), name);
      return this;
    }

    public HttpRequest AddFile(string name, string fileName, byte[] value)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (name));
      if (fileName == null)
        throw new ArgumentNullException(nameof (fileName));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.AddedMultipartData.Add((HttpContent) new BytesContent(value), name, fileName);
      return this;
    }

    public HttpRequest AddFile(
      string name,
      string fileName,
      string contentType,
      byte[] value)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (name));
      if (fileName == null)
        throw new ArgumentNullException(nameof (fileName));
      if (contentType == null)
        throw new ArgumentNullException(nameof (contentType));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.AddedMultipartData.Add((HttpContent) new BytesContent(value), name, fileName, contentType);
      return this;
    }

    public HttpRequest AddFile(string name, string fileName, Stream stream)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (name));
      if (fileName == null)
        throw new ArgumentNullException(nameof (fileName));
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      this.AddedMultipartData.Add((HttpContent) new StreamContent(stream, 32768), name, fileName);
      return this;
    }

    public HttpRequest AddFile(string name, string fileName, string path)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (name));
      if (fileName == null)
        throw new ArgumentNullException(nameof (fileName));
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      if (path.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (path));
      this.AddedMultipartData.Add((HttpContent) new FileContent(path, 32768), name, fileName);
      return this;
    }

    public HttpRequest AddFile(string name, string path)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (name));
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      if (path.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (path));
      this.AddedMultipartData.Add((HttpContent) new FileContent(path, 32768), name, Path.GetFileName(path));
      return this;
    }

    public HttpRequest AddHeader(string name, string value)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (name));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (value.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (value));
      if (this.IsClosedHeader(name))
        throw new ArgumentException(string.Format(Resources.ArgumentException_HttpRequest_SetNotAvailableHeader, (object) name), nameof (name));
      if (this.temporaryHeaders == null)
        this.temporaryHeaders = new Dictionary<string, string>();
      this.temporaryHeaders[name] = value;
      return this;
    }

    public HttpRequest AddHeader(HttpHeader header, string value)
    {
      this.AddHeader(Http.Headers[header], value);
      return this;
    }

    public void Close()
    {
      this.Dispose();
    }

    public void Dispose()
    {
      this.Dispose(true);
    }

    public bool ContainsCookie(string name)
    {
      if (this.Cookies == null)
        return false;
      return this.Cookies.ContainsKey(name);
    }

    public bool ContainsHeader(string headerName)
    {
      if (headerName == null)
        throw new ArgumentNullException(nameof (headerName));
      if (headerName.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (headerName));
      return this.permanentHeaders.ContainsKey(headerName);
    }

    public bool ContainsHeader(HttpHeader header)
    {
      return this.ContainsHeader(Http.Headers[header]);
    }

    public Dictionary<string, string>.Enumerator EnumerateHeaders()
    {
      return this.permanentHeaders.GetEnumerator();
    }

    public void ClearAllHeaders()
    {
      this.permanentHeaders.Clear();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.connection == null)
        return;
      this.connection.Close();
      this.connection = (TcpClient) null;
      this.connectionCommonStream = (Stream) null;
      this.connectionNetworkStream = (NetworkStream) null;
      this.keepAliveRequestCount = 0;
    }

    protected virtual void OnUploadProgressChanged(UploadProgressChangedEventArgs e)
    {
      EventHandler<UploadProgressChangedEventArgs> progressChangedHandler = this.uploadProgressChangedHandler;
      if (progressChangedHandler == null)
        return;
      progressChangedHandler((object) this, e);
    }

    protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
    {
      EventHandler<DownloadProgressChangedEventArgs> progressChangedHandler = this.downloadProgressChangedHandler;
      if (progressChangedHandler == null)
        return;
      progressChangedHandler((object) this, e);
    }

    private void Init()
    {
      this.KeepAlive = true;
      this.AllowAutoRedirect = true;
      this.EnableEncodingContent = true;
      this.response = new HttpResponse(this);
    }

    private Uri GetRequestAddress(Uri baseAddress, Uri address)
    {
      Uri result;
      if (baseAddress == (Uri) null)
        result = new UriBuilder(address.OriginalString).Uri;
      else
        Uri.TryCreate(baseAddress, address, out result);
      return result;
    }

    private HttpResponse Request(HttpMethod method, Uri address, HttpContent content)
    {
      this._method = method;
      this.content = content;
      this.CloseConnectionIfNeeded();
      Uri address1 = this.Address;
      this.Address = address;
      bool connectionOrUseExisting;
      try
      {
        connectionOrUseExisting = this.TryCreateConnectionOrUseExisting(address, address1);
      }
      catch (HttpException ex)
      {
        if (this.CanReconnect())
          return this.ReconnectAfterFail();
        throw;
      }
      if (connectionOrUseExisting)
        this.keepAliveRequestCount = 1;
      else
        ++this.keepAliveRequestCount;
      try
      {
        this.SendRequestData(method);
      }
      catch (SecurityException ex)
      {
        throw this.NewHttpException(Resources.HttpException_FailedSendRequest, (Exception) ex, HttpExceptionStatus.SendFailure);
      }
      catch (IOException ex)
      {
        if (this.CanReconnect())
          return this.ReconnectAfterFail();
        throw this.NewHttpException(Resources.HttpException_FailedSendRequest, (Exception) ex, HttpExceptionStatus.SendFailure);
      }
      try
      {
        this.ReceiveResponseHeaders(method);
      }
      catch (HttpException ex)
      {
        if (this.CanReconnect())
          return this.ReconnectAfterFail();
        if (this.KeepAlive && !this.keepAliveReconnected && (!connectionOrUseExisting && ex.EmptyMessageBody))
          return this.KeepAliveReconect();
        throw;
      }
      this.response.ReconnectCount = this.reconnectCount;
      this.reconnectCount = 0;
      this.keepAliveReconnected = false;
      this.whenConnectionIdle = DateTime.Now;
      if (!this.IgnoreProtocolErrors)
        this.CheckStatusCode(this.response.StatusCode);
      if (this.AllowAutoRedirect && this.response.HasRedirect)
      {
        if (++this.redirectionCount > this.maximumAutomaticRedirections)
          throw this.NewHttpException(Resources.HttpException_LimitRedirections, (Exception) null, HttpExceptionStatus.Other);
        this.ClearRequestData();
        return this.Request(HttpMethod.GET, this.response.RedirectAddress, (HttpContent) null);
      }
      this.redirectionCount = 0;
      return this.response;
    }

    private void CloseConnectionIfNeeded()
    {
      if (this.connection == null || this.response.HasError)
        return;
      if (this.response.MessageBodyLoaded)
        return;
      try
      {
        this.response.None();
      }
      catch (HttpException ex)
      {
        this.Dispose();
      }
    }

    private bool TryCreateConnectionOrUseExisting(Uri address, Uri previousAddress)
    {
      ProxyClient proxy = this.GetProxy();
      if (!((this.connection != null ? 1 : 0) == 0 | this.currentProxy != proxy | (previousAddress == (Uri) null || previousAddress.Port != address.Port || previousAddress.Host != address.Host || previousAddress.Scheme != address.Scheme)) && !this.response.HasError && !this.KeepAliveLimitIsReached())
        return false;
      this.currentProxy = proxy;
      this.Dispose();
      this.CreateConnection(address);
      return true;
    }

    private bool KeepAliveLimitIsReached()
    {
      if (!this.KeepAlive)
        return false;
      int? nullable = this.response.MaximumKeepAliveRequests;
      if (this.keepAliveRequestCount >= (nullable ?? this.maximumKeepAliveRequests))
        return true;
      nullable = this.response.KeepAliveTimeout;
      return this.whenConnectionIdle.AddMilliseconds((double) (nullable ?? this.keepAliveTimeout)) < DateTime.Now;
    }

    private void SendRequestData(HttpMethod method)
    {
      long contentLength = 0;
      string contentType = string.Empty;
      if (this.CanContainsRequestBody(method) && this.content != null)
      {
        contentType = this.content.ContentType;
        contentLength = this.content.CalculateContentLength();
      }
      string startingLine = this.GenerateStartingLine(method);
      string headers = this.GenerateHeaders(method, contentLength, contentType);
      byte[] bytes1 = Encoding.ASCII.GetBytes(startingLine);
      byte[] bytes2 = Encoding.ASCII.GetBytes(headers);
      this.bytesSent = 0L;
      this.totalBytesSent = (long) (bytes1.Length + bytes2.Length) + contentLength;
      this.connectionCommonStream.Write(bytes1, 0, bytes1.Length);
      this.connectionCommonStream.Write(bytes2, 0, bytes2.Length);
      if ((this.content == null ? 0 : (contentLength > 0L ? 1 : 0)) == 0)
        return;
      this.content.WriteTo(this.connectionCommonStream);
    }

    private void ReceiveResponseHeaders(HttpMethod method)
    {
      this.canReportBytesReceived = false;
      this.bytesReceived = 0L;
      this.totalBytesReceived = this.response.LoadResponse(method);
      this.canReportBytesReceived = true;
    }

    private bool CanReconnect()
    {
      if (this.Reconnect)
        return this.reconnectCount < this.reconnectLimit;
      return false;
    }

    private HttpResponse ReconnectAfterFail()
    {
      this.Dispose();
      Thread.Sleep(this.reconnectDelay);
      ++this.reconnectCount;
      return this.Request(this._method, this.Address, this.content);
    }

    private HttpResponse KeepAliveReconect()
    {
      this.Dispose();
      this.keepAliveReconnected = true;
      return this.Request(this._method, this.Address, this.content);
    }

    private void CheckStatusCode(HttpStatusCode statusCode)
    {
      int num = (int) statusCode;
      if (num >= 400 && num < 500)
        throw new HttpException(string.Format(Resources.HttpException_ClientError, (object) num), HttpExceptionStatus.ProtocolError, this.response.StatusCode, (Exception) null);
      if (num >= 500)
        throw new HttpException(string.Format(Resources.HttpException_SeverError, (object) num), HttpExceptionStatus.ProtocolError, this.response.StatusCode, (Exception) null);
    }

    private bool CanContainsRequestBody(HttpMethod method)
    {
      if (method != HttpMethod.PUT && method != HttpMethod.POST)
        return method == HttpMethod.DELETE;
      return true;
    }

    private ProxyClient GetProxy()
    {
      if (HttpRequest.DisableProxyForLocalAddress)
      {
        try
        {
          IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
          foreach (object hostAddress in Dns.GetHostAddresses(this.Address.Host))
          {
            if (hostAddress.Equals((object) ipAddress))
              return (ProxyClient) null;
          }
        }
        catch (Exception ex)
        {
          if (ex is SocketException || ex is ArgumentException)
            throw this.NewHttpException(Resources.HttpException_FailedGetHostAddresses, ex, HttpExceptionStatus.Other);
          throw;
        }
      }
      ProxyClient proxyClient = this.Proxy ?? HttpRequest.GlobalProxy;
      if (proxyClient == null && HttpRequest.UseIeProxy && !WinInet.InternetConnected)
        proxyClient = (ProxyClient) WinInet.IEProxy;
      return proxyClient;
    }

    private TcpClient CreateTcpConnection(string host, int port)
    {
      TcpClient tcpClient;
      if (this.currentProxy == null)
      {
        tcpClient = new TcpClient();
        Exception connectException = (Exception) null;
        ManualResetEventSlim connectDoneEvent = new ManualResetEventSlim();
        try
        {
          tcpClient.BeginConnect(host, port, (AsyncCallback) (ar =>
          {
            try
            {
              tcpClient.EndConnect(ar);
            }
            catch (Exception ex)
            {
              connectException = ex;
            }
            connectDoneEvent.Set();
          }), (object) tcpClient);
        }
        catch (Exception ex)
        {
          tcpClient.Close();
          if (ex is SocketException || ex is SecurityException)
            throw this.NewHttpException(Resources.HttpException_FailedConnect, ex, HttpExceptionStatus.ConnectFailure);
          throw;
        }
        if (!connectDoneEvent.Wait(this.connectTimeout))
        {
          tcpClient.Close();
          throw this.NewHttpException(Resources.HttpException_ConnectTimeout, (Exception) null, HttpExceptionStatus.ConnectFailure);
        }
        if (connectException != null)
        {
          tcpClient.Close();
          if (connectException is SocketException)
            throw this.NewHttpException(Resources.HttpException_FailedConnect, connectException, HttpExceptionStatus.ConnectFailure);
          throw connectException;
        }
        if (!tcpClient.Connected)
        {
          tcpClient.Close();
          throw this.NewHttpException(Resources.HttpException_FailedConnect, (Exception) null, HttpExceptionStatus.ConnectFailure);
        }
        tcpClient.SendTimeout = this.readWriteTimeout;
        tcpClient.ReceiveTimeout = this.readWriteTimeout;
      }
      else
      {
        try
        {
          tcpClient = this.currentProxy.CreateConnection(host, port, (TcpClient) null);
        }
        catch (ProxyException ex)
        {
          throw this.NewHttpException(Resources.HttpException_FailedConnect, (Exception) ex, HttpExceptionStatus.ConnectFailure);
        }
      }
      return tcpClient;
    }

    private void CreateConnection(Uri address)
    {
      this.connection = this.CreateTcpConnection(address.Host, address.Port);
      this.connectionNetworkStream = this.connection.GetStream();
      if (address.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
      {
        try
        {
          SslStream sslStream = this.SslCertificateValidatorCallback != null ? new SslStream((Stream) this.connectionNetworkStream, false, this.SslCertificateValidatorCallback) : new SslStream((Stream) this.connectionNetworkStream, false, Http.AcceptAllCertificationsCallback);
          sslStream.AuthenticateAsClient(address.Host);
          this.connectionCommonStream = (Stream) sslStream;
        }
        catch (Exception ex)
        {
          if (ex is IOException || ex is AuthenticationException)
            throw this.NewHttpException(Resources.HttpException_FailedSslConnect, ex, HttpExceptionStatus.ConnectFailure);
          throw;
        }
      }
      else
        this.connectionCommonStream = (Stream) this.connectionNetworkStream;
      if (this.uploadProgressChangedHandler == null && this.downloadProgressChangedHandler == null)
        return;
      HttpRequest.HttpWraperStream httpWraperStream = new HttpRequest.HttpWraperStream(this.connectionCommonStream, this.connection.SendBufferSize);
      if (this.uploadProgressChangedHandler != null)
        httpWraperStream.BytesWriteCallback = new Action<int>(this.ReportBytesSent);
      if (this.downloadProgressChangedHandler != null)
        httpWraperStream.BytesReadCallback = new Action<int>(this.ReportBytesReceived);
      this.connectionCommonStream = (Stream) httpWraperStream;
    }

    private string GenerateStartingLine(HttpMethod method)
    {
      string str = this.currentProxy == null || this.currentProxy.Type != ProxyType.Http && this.currentProxy.Type != ProxyType.Chain ? this.Address.PathAndQuery : this.Address.AbsoluteUri;
      return string.Format("{0} {1} HTTP/{2}\r\n", (object) method, (object) str, (object) HttpRequest.ProtocolVersion);
    }

    private string GenerateHeaders(HttpMethod method, long contentLength = 0, string contentType = null)
    {
      Dictionary<string, string> commonHeaders = this.GenerateCommonHeaders(method, contentLength, contentType);
      this.MergeHeaders(commonHeaders, this.permanentHeaders);
      if (this.temporaryHeaders != null && this.temporaryHeaders.Count > 0)
        this.MergeHeaders(commonHeaders, this.temporaryHeaders);
      if (this.Cookies != null && this.Cookies.Count != 0 && !commonHeaders.ContainsKey("Cookie"))
        commonHeaders["Cookie"] = this.Cookies.ToString();
      return this.ToHeadersString(commonHeaders);
    }

    private Dictionary<string, string> GenerateCommonHeaders(
      HttpMethod method,
      long contentLength = 0,
      string contentType = null)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      dictionary["Host"] = !this.Address.IsDefaultPort ? string.Format("{0}:{1}", (object) this.Address.Host, (object) this.Address.Port) : this.Address.Host;
      HttpProxyClient httpProxy = (HttpProxyClient) null;
      if (this.currentProxy != null && this.currentProxy.Type == ProxyType.Http)
        httpProxy = this.currentProxy as HttpProxyClient;
      else if (this.currentProxy != null && this.currentProxy.Type == ProxyType.Chain)
        httpProxy = this.FindHttpProxyInChain(this.currentProxy as ChainProxyClient);
      if (httpProxy != null)
      {
        dictionary["Proxy-Connection"] = !this.KeepAlive ? "close" : "keep-alive";
        if (!string.IsNullOrEmpty(httpProxy.Username) || !string.IsNullOrEmpty(httpProxy.Password))
          dictionary["Proxy-Authorization"] = this.GetProxyAuthorizationHeader(httpProxy);
      }
      else
        dictionary["Connection"] = !this.KeepAlive ? "close" : "keep-alive";
      if (!string.IsNullOrEmpty(this.Username) || !string.IsNullOrEmpty(this.Password))
        dictionary["Authorization"] = this.GetAuthorizationHeader();
      if (this.EnableEncodingContent)
        dictionary["Accept-Encoding"] = "gzip,deflate";
      if (this.Culture != null)
        dictionary["Accept-Language"] = this.GetLanguageHeader();
      if (this.CharacterSet != null)
        dictionary["Accept-Charset"] = this.GetCharsetHeader();
      if (this.CanContainsRequestBody(method))
      {
        if (contentLength > 0L)
          dictionary["Content-Type"] = contentType;
        dictionary["Content-Length"] = contentLength.ToString();
      }
      return dictionary;
    }

    private string GetAuthorizationHeader()
    {
      return string.Format("Basic {0}", (object) Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", (object) this.Username, (object) this.Password))));
    }

    private string GetProxyAuthorizationHeader(HttpProxyClient httpProxy)
    {
      return string.Format("Basic {0}", (object) Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", (object) httpProxy.Username, (object) httpProxy.Password))));
    }

    private string GetLanguageHeader()
    {
      string str = this.Culture == null ? CultureInfo.CurrentCulture.Name : this.Culture.Name;
      if (str.StartsWith("en"))
        return str;
      return string.Format("{0},{1};q=0.8,en-US;q=0.6,en;q=0.4", (object) str, (object) str.Substring(0, 2));
    }

    private string GetCharsetHeader()
    {
      if (this.CharacterSet == Encoding.UTF8)
        return "utf-8;q=0.7,*;q=0.3";
      return string.Format("{0},utf-8;q=0.7,*;q=0.3", this.CharacterSet != null ? (object) this.CharacterSet.WebName : (object) Encoding.Default.WebName);
    }

    private void MergeHeaders(
      Dictionary<string, string> destination,
      Dictionary<string, string> source)
    {
      foreach (KeyValuePair<string, string> keyValuePair in source)
        destination[keyValuePair.Key] = keyValuePair.Value;
    }

    private HttpProxyClient FindHttpProxyInChain(ChainProxyClient chainProxy)
    {
      HttpProxyClient httpProxyClient = (HttpProxyClient) null;
      foreach (ProxyClient proxy in chainProxy.Proxies)
      {
        if (proxy.Type == ProxyType.Http)
        {
          httpProxyClient = proxy as HttpProxyClient;
          if (!string.IsNullOrEmpty(httpProxyClient.Username) || !string.IsNullOrEmpty(httpProxyClient.Password))
            return httpProxyClient;
        }
        else if (proxy.Type == ProxyType.Chain)
        {
          HttpProxyClient httpProxyInChain = this.FindHttpProxyInChain(proxy as ChainProxyClient);
          if (httpProxyInChain != null && (!string.IsNullOrEmpty(httpProxyInChain.Username) || !string.IsNullOrEmpty(httpProxyInChain.Password)))
            return httpProxyInChain;
        }
      }
      return httpProxyClient;
    }

    private string ToHeadersString(Dictionary<string, string> headers)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, string> header in headers)
        stringBuilder.AppendFormat("{0}: {1}\r\n", (object) header.Key, (object) header.Value);
      stringBuilder.AppendLine();
      return stringBuilder.ToString();
    }

    private void ReportBytesSent(int bytesSent)
    {
      this.bytesSent += (long) bytesSent;
      this.OnUploadProgressChanged(new UploadProgressChangedEventArgs(this.bytesSent, this.totalBytesSent));
    }

    private void ReportBytesReceived(int bytesReceived)
    {
      this.bytesReceived += (long) bytesReceived;
      if (!this.canReportBytesReceived)
        return;
      this.OnDownloadProgressChanged(new DownloadProgressChangedEventArgs(this.bytesReceived, this.totalBytesReceived));
    }

    private bool IsClosedHeader(string name)
    {
      return HttpRequest.closedHeaders.Contains<string>(name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    private void ClearRequestData()
    {
      this.content = (HttpContent) null;
      this.temporaryUrlParams = (RequestParams) null;
      this.temporaryParams = (RequestParams) null;
      this.temporaryMultipartContent = (MultipartContent) null;
      this.temporaryHeaders = (Dictionary<string, string>) null;
    }

    private HttpException NewHttpException(
      string message,
      Exception innerException = null,
      HttpExceptionStatus status = HttpExceptionStatus.Other)
    {
      return new HttpException(string.Format(message, (object) this.Address.Host), status, HttpStatusCode.None, innerException);
    }

    private sealed class HttpWraperStream : Stream
    {
      private readonly Stream baseStream;
      private readonly int sendBufferSize;

      public Action<int> BytesReadCallback { get; set; }

      public Action<int> BytesWriteCallback { get; set; }

      public override bool CanRead
      {
        get
        {
          return this.baseStream.CanRead;
        }
      }

      public override bool CanSeek
      {
        get
        {
          return this.baseStream.CanSeek;
        }
      }

      public override bool CanTimeout
      {
        get
        {
          return this.baseStream.CanTimeout;
        }
      }

      public override bool CanWrite
      {
        get
        {
          return this.baseStream.CanWrite;
        }
      }

      public override long Length
      {
        get
        {
          return this.baseStream.Length;
        }
      }

      public override long Position
      {
        get
        {
          return this.baseStream.Position;
        }
        set
        {
          this.baseStream.Position = value;
        }
      }

      public HttpWraperStream(Stream baseStream, int sendBufferSize)
      {
        this.baseStream = baseStream;
        this.sendBufferSize = sendBufferSize;
      }

      public override void Flush()
      {
      }

      public override void SetLength(long value)
      {
        this.baseStream.SetLength(value);
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        return this.baseStream.Seek(offset, origin);
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        int num = this.baseStream.Read(buffer, offset, count);
        Action<int> bytesReadCallback = this.BytesReadCallback;
        if (bytesReadCallback != null)
          bytesReadCallback(num);
        return num;
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        if (this.BytesWriteCallback == null)
        {
          this.baseStream.Write(buffer, offset, count);
        }
        else
        {
          int offset1 = 0;
          while (count > 0)
          {
            int count1;
            if (count >= this.sendBufferSize)
            {
              count1 = this.sendBufferSize;
              this.baseStream.Write(buffer, offset1, count1);
              offset1 += this.sendBufferSize;
              count -= this.sendBufferSize;
            }
            else
            {
              count1 = count;
              this.baseStream.Write(buffer, offset1, count1);
              count = 0;
            }
            this.BytesWriteCallback(count1);
          }
        }
      }
    }
  }
}
